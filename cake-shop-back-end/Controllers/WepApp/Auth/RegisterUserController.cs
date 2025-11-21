using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.Controllers.WepApp.Auth;

[Route("api/auth")]
[AllowAnonymous]
[ApiController]
public class RegisterUserController(IUser _user,
    ILoggingHelpers _loggingHelpers,
    AppDbContext _context,
    ICommonFunction _commonFunction,
    IJwtAuth _jwtAuth
    ) : ControllerBase
{
    [Route("register-user")]
    [HttpPost]
    public async Task<JsonResult> RegisterUser(UserRequest request)
    {
        APIResponse data = await _user.CreateAccountAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/auth/register-user",
            Actions = "Tạo mới người dùng",
            Application = "WEB ADMIN",
            Content = "Tạo mới người dùng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = "New Customer",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("userLogin")]
    [HttpPost]
    public async Task<JsonResult> UserLogin(LoginRequest loginRequest)
    {
        // Check Username
        var checkUserName = await _context.Users.Where(x => x.username == loginRequest.Username && x.is_admin == false && x.status == 1).FirstOrDefaultAsync();

        if (checkUserName == null)
        {
            return new JsonResult(new APIResponse("ERROR_USERNAME_NOT_EXISTS")) { StatusCode = 200 };
        }

        if (string.IsNullOrEmpty(loginRequest.Password) || string.IsNullOrEmpty(loginRequest.Username))
        {
            return new JsonResult(new APIResponse("ERROR_PASSWORD_USERNAME_EMPTY")) { StatusCode = 200 };
        }

        if (checkUserName.password != _commonFunction.ComputeSha256Hash(loginRequest.Password))
        {
            return new JsonResult(new APIResponse("ERROR_PASSWORD_INCORRECT")) { StatusCode = 200 };
        }

        var token = _jwtAuth.Authentication(loginRequest.Username, _commonFunction.ComputeSha256Hash(loginRequest.Password), Consts.USER_TYPE_WEB_USER);

        if (token == null)
        {
            return new JsonResult(new APIResponse("ERROR_SERVER")) { StatusCode = 200 };
        }

        var listFunctionIds = await (from p in _context.Actions
                                     join up in _context.UserPermissions on p.id equals up.action_id
                                     join f in _context.Functions on p.function_id equals f.id
                                     where up.user_id == checkUserName.id
                                     select f.id).ToListAsync();

        var user_permissions = await (from p in _context.Functions
                                      where p.status == 1 && listFunctionIds.Contains(p.id)
                                      select new
                                      {
                                          id = p.id,
                                          function_code = p.code,
                                          function_name = p.name,
                                          path = p.url,
                                          actions = (from i in _context.Actions
                                                     join up in _context.UserPermissions on i.id equals up.action_id
                                                     where i.function_id == p.id && up.user_id == checkUserName.id
                                                     select new
                                                     {
                                                         action_id = i.id,
                                                         action_code = i.code,
                                                         action_name = i.name,
                                                         path = i.url
                                                     }).ToList()
                                      }).ToListAsync();

        // Login Response
        object loginResponse = new
        {
            token = token,
            user_id = checkUserName.id,
            username = checkUserName.username,
            full_name = checkUserName.full_name,
            avatar = checkUserName.avatar,
            is_admin = checkUserName.is_admin,
            is_sysadmin = checkUserName.is_sysadmin,
            user_permissions = user_permissions,
            group_id = checkUserName.user_group_id
        };

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        // open transaction 

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            checkUserName.last_login = DateTime.Now;

            await _context.SaveChangesAsync();

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "/api/auth/userLogin",
                Actions = "Đăng nhập",
                Application = "WEB STORE",
                Content = loginResponse.ToString(),
                Functions = "Hệ thống",
                IsLogin = true,
                ResultLogging = "Thành công",
                UserCreated = checkUserName.username,
                IP = remoteIP
            });

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new JsonResult(new APIResponse("ERROR_" + ex.Message.ToUpper())) { StatusCode = 200 };
        }

        return new JsonResult(new APIResponse(loginResponse)) { StatusCode = 200 };
    }



}
