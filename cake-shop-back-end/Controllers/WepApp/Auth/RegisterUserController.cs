using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp.Auth;

[Route("api/auth")]
[AllowAnonymous]
[ApiController]
public class RegisterUserController(IUser _user , ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("register-user")]
    [HttpPost]
    public async Task<JsonResult> RegisterUser(UserRequest request)
    {
        APIResponse data = await _user.CreateNomalUser(request);

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


}
