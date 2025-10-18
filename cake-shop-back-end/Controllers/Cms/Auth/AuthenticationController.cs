using System.Security.Claims;
using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Models.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.Controllers.Cms.Auth;

[Route("api/auth")]
[ApiController]
public class AuthenticationController(
    IJwtAuth _jwtAuth,
    ILoggingHelpers _loggingHelpers,
    IEmailSender _emailSender,
    ICommonFunction _commonFunction,
    AppDbContext _context
    ) : ControllerBase
{
    [AllowAnonymous]
    [Route("login")]
    [HttpPost]

    public async Task<JsonResult> Login(LoginRequest loginRequest)
    {
        var checkUserName = await _context.Users.Where(x => x.username == loginRequest.Username &&  x.is_admin != true && x.status == 1).FirstOrDefaultAsync();

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

        object loginResponse = new
        {
            token = token,
            user_id = checkUserName.id,
            username = checkUserName.username,
            full_name = checkUserName.full_name,
            avatar = checkUserName.avatar,
            is_admin = checkUserName.is_admin,
            is_sysadmin = checkUserName.is_sysadmin,
        };

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_USER,
            IsCallApi = true,
            ApiName = "/api/auth/login",
            Actions = "Đăng nhập",
            Application = "WEB ADMIN",
            Content = loginResponse.ToString(),
            Functions = "Hệ thống",
            IsLogin = true,
            ResultLogging = "Thành công",
            UserCreated = checkUserName.username,
            IP = remoteIP
        });

        return new JsonResult(new APIResponse(loginResponse)) { StatusCode = 200 };
    }


    [AllowAnonymous]
    [Route("adminLogin")]
    [HttpPost]
    public async Task<JsonResult> AdminLogin(LoginRequest loginRequest)
    {
        // Check Username
        var checkUserName = await _context.Users.Where(x => x.username == loginRequest.Username && x.is_admin == true && x.status == 1).FirstOrDefaultAsync();
        
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

        var token = _jwtAuth.Authentication(loginRequest.Username, _commonFunction.ComputeSha256Hash(loginRequest.Password), Consts.USER_TYPE_WEB_ADMIN);
        
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

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "/api/auth/adminLogin",
            Actions = "Đăng nhập",
            Application = "WEB ADMIN",
            Content = loginResponse.ToString(),
            Functions = "Hệ thống",
            IsLogin = true,
            ResultLogging = "Thành công",
            UserCreated = checkUserName.username,
            IP = remoteIP
        });

        return new JsonResult(new APIResponse(loginResponse)) { StatusCode = 200 };
    }

    [Route("adminChangePass")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> AdminChangePassword(PasswordRequest pwdRequest)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var user = await _context.Users.Where(x => x.username == username.Value && x.is_admin == true).FirstOrDefaultAsync();

        if (pwdRequest.OldPassword == null)
        {
            return new JsonResult(new APIResponse("ERROR_OLD_PASSWORD_MISSING")) { StatusCode = 200 };
        }

        if (pwdRequest.NewPassword == null)
        {
            return new JsonResult(new APIResponse("ERROR_NEW_PASSWORD_MISSING")) { StatusCode = 200 };
        }

        if (user == null)
        {
            return new JsonResult(new APIResponse("ERROR_USER_NOT_EXISTS")) { StatusCode = 200 };
        }

        if (user.password != _commonFunction.ComputeSha256Hash(pwdRequest.OldPassword))
        {
            return new JsonResult(new APIResponse(" ")) { StatusCode = 200 };
        }

        try
        {
            user.password = _commonFunction.ComputeSha256Hash(pwdRequest.NewPassword);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new JsonResult(new APIResponse("ERROR_CHANGE_PASS_FAIL")) { StatusCode = 200 };
        }

        // Ghi log (Đã cập nhật)
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/auth/adminChangePass",
            Actions = "Đổi mật khẩu",
            Application = "WEB ADMIN",
            Content = "Đổi mật khẩu",
            Functions = "Hệ thống",
            IsLogin = true,
            ResultLogging = "Thành công",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(new APIResponse(200)) { StatusCode = 200 };
    }

    [Route("adminGetUserInfo")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpGet]
    public async Task<JsonResult> AdminGetUserInfo()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var user = await _context.Users.Where(x => x.username == username.Value && x.is_admin == true).FirstOrDefaultAsync();

        if (user == null)
        {
            return new JsonResult(new APIResponse("ERROR_USER_NOT_EXISTS")) { StatusCode = 200 };
        }

        var userGroup = new UserGroup();
        if (user.user_group_id != null)
        {
            userGroup = await _context.UserGroups.Where(x => x.id == user.user_group_id).FirstOrDefaultAsync();
        }

        var listFunctionIds = await (from p in _context.Actions
                                     join up in _context.UserPermissions on p.id equals up.action_id
                                     join f in _context.Functions on p.function_id equals f.id
                                     where up.user_id == user.id
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
                                                     where i.function_id == p.id && up.user_id == user.id
                                                     select new
                                                     {
                                                         action_id = i.id,
                                                         action_code = i.code,
                                                         action_name = i.name,
                                                         path = i.url
                                                     }).ToList() // ToList() ở đây để tránh EF Core báo lỗi trong truy vấn chính
                                      }).ToListAsync();

        var userInfoResponse = new
        {
            id = user.id,
            address = user.address,
            avatar = user.avatar,
            email = user.email,
            full_name = user.full_name,
            phone = user.phone,
            username = user.username,
            user_group_id = user.user_group_id,
            user_group_name = userGroup == null ? "" : userGroup.name,
            is_sysadmin = user.is_sysadmin,
            is_admin = user.is_admin,
            user_permissions = user_permissions
        };

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/auth/adminGetUserInfo",
            Actions = "Lấy thông tin cá nhân",
            Application = "WEB ADMIN",
            Content = $"Lấy thông tin cá nhân user: {username?.Value ?? "Unknown"}",
            Functions = "Hệ thống",
            IsLogin = false,
            ResultLogging = "Thành công",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(new APIResponse(userInfoResponse)) { StatusCode = 200 };
    }

    [Route("adminUpdateUserInfo")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> AdminUpdateUserInfo(User userRequest)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var user = await _context.Users.Where(x => x.username == username.Value && x.is_admin == true).FirstOrDefaultAsync();

        if (user == null)
        {
            return new JsonResult(new APIResponse("ERROR_USER_NOT_EXISTS")) { StatusCode = 200 };
        }

        try
        {
            user.full_name = userRequest.full_name;
            user.email = userRequest.email;
            user.phone = userRequest.phone;
            user.address = userRequest.address;
            user.avatar = userRequest.avatar;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new JsonResult(new APIResponse(400)) { StatusCode = 200 };
        }

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/auth/adminUpdateUserInfo",
            Actions = "Cập nhật thông tin cá nhân",
            Application = "WEB ADMIN",
            Content = $"Cập nhật thông tin cá nhân user: {username?.Value ?? "Unknown"}",
            Functions = "Hệ thống",
            IsLogin = false,
            ResultLogging = "Thành công",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(new APIResponse(200)) { StatusCode = 200 };
    }

}
