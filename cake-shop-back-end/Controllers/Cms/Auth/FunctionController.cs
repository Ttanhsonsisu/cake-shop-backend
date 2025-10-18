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

[Route("api/function")]
[Authorize(Policy = "WebAdminUser")]
[ApiController]
public class FunctionController(IFunction _function, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(FunctionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _function.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/function/list",
            Actions = "Danh sách Chức năng",
            Application = "WEB ADMIN",
            Content = "Danh sách Chức năng",
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

        APIResponse data = await _function.GetDetaiAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/function/" + id,
            Actions = "Chi tiết Chức năng",
            Application = "WEB ADMIN",
            Content = "Chi tiết Chức năng",
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
    public async Task<JsonResult> Create(FunctionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _function.CreateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/function/create",
            Actions = "Tạo mới Chức năng",
            Application = "WEB ADMIN",
            Content = "Tạo mới Chức năng",
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
    public async Task<JsonResult> Update(FunctionRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _function.UpdateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/function/update",
            Actions = "Cập nhật Chức năng",
            Application = "WEB ADMIN",
            Content = "Cập nhật Chức năng",
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
    public async Task<JsonResult> Delete(FunctionRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _function.DeleteAsync(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/function/delete",
            Actions = "Xóa Chức năng",
            Application = "WEB ADMIN",
            Content = "Xóa Chức năng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("lstFunctionPermission")]
    [HttpGet]
    public async Task<JsonResult> GetListFunctionPermission()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _function.GetListFunctionPermissionAsync();

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/function/lstFunctionPermission",
                Actions = "Lấy danh sách chức năng phân quyền",
                Application = "WEB ADMIN",
                Content = "Lấy danh sách chức năng phân quyền",
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
    public async Task<JsonResult> ChangeStatus(FunctionRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _function.ChangeStatusAsync(req);
        // Ghi log
        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/function/changeStatus",
                Actions = "Đổi trạng thái Chức năng",
                Application = "WEB ADMIN",
                Content = "Đổi trạng thái Chức năng",
                Functions = "Danh mục",
                IsLogin = false,
                ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
                UserCreated = username?.Value ?? "Unknown",
                IP = remoteIP
            });
        }

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("getFunctionTree")]
    [HttpGet]
    public async Task<JsonResult> getFunctionTree()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _function.GetFunctionTreeAsync();

        if (data.Code == "200")
        {
            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _loggingHelpers.InsertLogging(new LoggingRequest
            {
                UserType = Consts.USER_TYPE_WEB_ADMIN,
                IsCallApi = true,
                ApiName = "api/function/getFunctionTree",
                Actions = "Lấy danh sách chức năng phân quyền",
                Application = "WEB ADMIN",
                Content = "Lấy danh sách chức năng phân quyền",
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
