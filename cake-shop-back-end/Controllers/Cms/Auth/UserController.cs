using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Auth;

[Route("api/user")]
[Authorize(Policy = "WebAdminUser")]
[ApiController]
public class UserController(IUser _user, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(UserRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _user.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/user/list",
            Actions = "Danh sách Tài khoản",
            Application = "WEB ADMIN",
            Content = "Danh sách Tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpGet("{id}")]
    public async Task<JsonResult> GetDetail(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _user.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/user/" + id,
            Actions = "Chi tiết Tài khoản",
            Application = "WEB ADMIN",
            Content = "Chi tiết Tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> Create(UserRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _user.CreateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/user/create",
            Actions = "Chi tiết Tài khoản",
            Application = "WEB ADMIN",
            Content = "Tạo mới Tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("update")]
    [HttpPost]
    public async Task<JsonResult> Update(UserRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _user.UpdateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/user/update",
            Actions = "Cập nhật Tài khoản",
            Application = "WEB ADMIN",
            Content = "Cập nhật Tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete(UserRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _user.DeleteAsync(req);
        
        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/user/delete",
                Actions = "Xóa Tài khoản",
                Application = "WEB ADMIN",
                Content = "Xóa Tài khoản",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> changeStatus(UserRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _user.ChangeStatusAsync(req);
       
        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/user/changeStatus",
                Actions = "Đổi trạng thái Nhân viên",
                Application = "WEB ADMIN",
                Content = "Đổi trạng thái Nhân viên",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }

        return new JsonResult(data) { StatusCode = 200 };
    }
}
