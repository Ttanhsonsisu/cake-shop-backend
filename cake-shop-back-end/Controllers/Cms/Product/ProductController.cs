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


[Route("api/cms/product")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class ProductController(IProductCake _productCake, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> CreateAsync(ProductCakeRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _productCake.CreateDraftAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/create",
            Actions = "Tạo mới sản phẩm",
            Application = "WEB ADMIN",
            Content = "Tạo mới sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("publishProduct")]
    [HttpPost]
    public async Task<JsonResult> PublishProduct(ProductCakeRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _productCake.PublishProductAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/publishProduct",
            Actions = "Phát hành sản phẩm mới",
            Application = "WEB ADMIN",
            Content = "Phát hành sản phẩm mới",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(ProductCakeRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _productCake.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/create",
            Actions = "Tạo mới sản phẩm",
            Application = "WEB ADMIN",
            Content = "Tạo mới sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpGet("{id}")]
    public async Task<JsonResult> GetList(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _productCake.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/" + id,
            Actions = "Chi tiết sản phẩm",
            Application = "WEB ADMIN",
            Content = "Chi tiết sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpPost("addImage")]
    public async Task<JsonResult> AddImage(ProductImageRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _productCake.AddProductGalleryAsync(request, username.Value);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/addImage",
            Actions = "Thêm ảnh sản phẩm",
            Application = "WEB ADMIN",
            Content = "Thêm ảnh sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpPost("addImages")]
    public async Task<JsonResult> AddImages(List<ProductImageRequest> request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _productCake.AddProductGallerysAsync(request, username.Value);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/addImages",
            Actions = "Thêm nhiều ảnh sản phẩm",
            Application = "WEB ADMIN",
            Content = "Thêm nhiều ảnh sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpPost("deleteImage")]
    public async Task<JsonResult> DeleteImage(ProductImageRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _productCake.RemoveProductGalleryAsync(request, username.Value);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/deleteImage",
            Actions = "xóa ảnh sản phẩm",
            Application = "WEB ADMIN",
            Content = "xóa ảnh sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpPost("deleteImages")]
    public async Task<JsonResult> DeleteImages([FromBody] List<long> request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        APIResponse data = await _productCake.RemoveProductGallerysAsync(request, username.Value);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/product/deleteImages",
            Actions = "xóa nhiều ảnh sản phẩm",
            Application = "WEB ADMIN",
            Content = "xóa nhiều ảnh sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }
}
