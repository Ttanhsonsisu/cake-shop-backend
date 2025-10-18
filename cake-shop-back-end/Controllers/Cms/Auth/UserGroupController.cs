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

[Route("api/usergroup")]
[ApiController]
public class UserGroupController(IUserGroup _userGroup, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> GetList(UserGroupRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/usergroup/list",
            Actions = "Danh sách Nhóm tài khoản",
            Application = "WEB ADMIN",
            Content = "Danh sách Nhóm tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Authorize(Policy = "WebAdminUser")]
    [HttpGet("{id}")]
    public async Task<JsonResult> GetDetail(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = $"api/usergroup/{id}",
            Actions = "Chi tiết Nhóm tài khoản",
            Application = "WEB ADMIN",
            Content = "Chi tiết Nhóm tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }


    [Route("create")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> Create(UserGroupRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.CreateAsync(request, username?.Value);

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/usergroup/create",
                Actions = "Tạo mới Nhóm tài khoản",
                Application = "WEB ADMIN",
                Content = "Tạo mới Nhóm tài khoản",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("update")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> Update(UserGroupRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.UpdateAsync(request, username?.Value);

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/usergroup/update",
                Actions = "Cập nhật Nhóm tài khoản",
                Application = "WEB ADMIN",
                Content = "Cập nhật Nhóm tài khoản",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }
        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [Authorize(Policy = "WebAdminUser")]
    [HttpPost]
    public async Task<JsonResult> Delete(UserGroupRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.DeleteAsync(req);

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/usergroup/delete",
                Actions = "Xóa Nhóm tài khoản",
                Application = "WEB ADMIN",
                Content = "Xóa Nhóm tài khoản",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Authorize]
    [HttpGet("permission/{id}")]
    public async Task<JsonResult> GetPermission(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.GetPermissionAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = $"api/usergroup/permission/{id}",
            Actions = "Lấy danh sách phân quyền Nhóm tài khoản",
            Application = "WEB ADMIN",
            Content = "Lấy danh sách phân quyền Nhóm tài khoản",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus(UserGroupRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _userGroup.ChangeStatusAsync(req);

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/usergroup/changeStatus",
                Actions = "Đổi trạng thái Nhóm tài khoản",
                Application = "WEB ADMIN",
                Content = "Đổi trạng thái Nhóm tài khoản",
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
