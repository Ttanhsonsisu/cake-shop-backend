using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Common;

[Route("api/logging")]
[Authorize(Policy = "WebAdminUser")]
[ApiController]
public class LoggingController(ILogging _logging , ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("listLogin")]
    [HttpPost]
    public async Task<JsonResult> GetListLogIn([FromBody] FilterLoggingRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _logging.GetListLogIn(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/logging/listLogin",
            Actions = "Danh sách Log Đăng nhập",
            Application = "WEB ADMIN",
            Content = "Danh sách Log Đăng nhập",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200};
    }

    [Route("listAction")]
    [HttpPost]
    public async Task<JsonResult> getListAction(FilterLoggingRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _logging.GetListAction(request);

        // write log
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/logging/listLogin",
            Actions = "Danh sách Log Đăng nhập",
            Application = "WEB ADMIN",
            Content = "Danh sách Log Đăng nhập",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("listCallApi")]
    [HttpPost]
    public async Task<JsonResult> getListCallApi(FilterLoggingRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _logging.GetListCallApi(request);

        // write log
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/logging/listLogin",
            Actions = "Danh sách Log Đăng nhập",
            Application = "WEB ADMIN",
            Content = "Danh sách Log Đăng nhập",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
