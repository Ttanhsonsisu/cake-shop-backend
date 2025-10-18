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

[Route("api/action")]
[Authorize(Policy = "WebAdminUser")]
[ApiController]
public class ActionController(IAction _action, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(ActionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _action.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/action/list",
            Actions = "Danh sách Hành động",
            Application = "WEB ADMIN",
            Content = "Danh sách Hành động",
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
        APIResponse data = await _action.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/action/" + id,
            Actions = "Chi tiết Hành động",
            Application = "WEB ADMIN",
            Content = "Chi tiết Hành động",
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
    public async Task<JsonResult> Create(ActionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _action.CreateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/action/create",
            Actions = "Tạo mới Hành động",
            Application = "WEB ADMIN",
            Content = "Tạo mới Hành động",
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
    public async Task<JsonResult> Update(ActionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _action.UpdateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/action/update",
            Actions = "Cập nhật Hành động",
            Application = "WEB ADMIN",
            Content = "Cập nhật Hành động",
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
    public async Task<JsonResult> Delete(ActionRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _action.DeleteAsync(req);
        // Ghi log
        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/action/delete",
                Actions = "Xóa Hành động",
                Application = "WEB ADMIN",
                Content = "Xóa Hành động",
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
    public async Task<JsonResult> ChangeStatus(ActionRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _action.ChangeStatusAsync(req);

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/action/changeStatus",
                Actions = "Đổi trạng thái Hành động",
                Application = "WEB ADMIN",
                Content = "Đổi trạng thái Hành động",
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
