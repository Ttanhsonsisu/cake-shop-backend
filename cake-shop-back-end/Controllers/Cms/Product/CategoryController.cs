using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Product;

[Route("api/cms/category")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class CategoryController(ILoggingHelpers _loggingHelpers, ICategory _category) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(CategoryRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/list",
            Actions = "Danh sách danh mục",
            Application = "WEB ADMIN",
            Content = "Danh sách danh mục",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpGet("{id}")]
    public async Task<JsonResult> Detail(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/" + id,
            Actions = "Chi tiết danh mục sản phẩm",
            Application = "WEB ADMIN",
            Content = "Chi tiết danh mục sản phẩm",
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
    public async Task<JsonResult> Create(CategoryRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.CreateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/create",
            Actions = "Tạo mới danh mục sản phẩm",
            Application = "WEB ADMIN",
            Content = "Tạo mới danh mục sản phẩm",
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
    public async Task<JsonResult> Update(CategoryRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.UpdateAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/create",
            Actions = "Cập nhật danh mục sản phẩm",
            Application = "WEB ADMIN",
            Content = "Cập nhật danh mục sản phẩm",
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
    public async Task<JsonResult> ChangeStatus(CategoryRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.ChangeStatusAsync(request, username.Value, request.TypeStatusChange ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/create",
            Actions = "Cập nhật trạng thái danh mục sản phẩm",
            Application = "WEB ADMIN",
            Content = "Cập nhật trạng thái danh mục sản phẩm",
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
    public async Task<JsonResult> Delete(CategoryRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _category.DeleteAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/category/create",
            Actions = "Xóa danh mục sản phẩm",
            Application = "WEB ADMIN",
            Content = "Xóa danh mục sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

}
