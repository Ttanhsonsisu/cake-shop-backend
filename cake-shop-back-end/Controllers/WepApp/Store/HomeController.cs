using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp.Store;

[Route("api/client/home")]
[ApiController]
[AllowAnonymous]
public class HomeController(IHome _homeClient, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("banners")]
    [HttpGet]
    public async Task<JsonResult> GetBanners()
    {
        // Lấy thông tin user nếu họ đã đăng nhập (nếu chưa thì null)
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _homeClient.GetBanners();

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER", // Hoặc Consts.USER_TYPE_CUSTOMER
            IsCallApi = true,
            ApiName = "api/client/home/banners",
            Actions = "Lấy Banner trang chủ",
            Application = "CLIENT WEB",
            Content = "Lấy danh sách Banner hiển thị",
            Functions = "Trang chủ",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest", // Nếu không login thì là Guest
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("categories")]
    [HttpGet]
    public async Task<JsonResult> GetCategories()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _homeClient.GetCategories();

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER",
            IsCallApi = true,
            ApiName = "api/client/home/categories",
            Actions = "Lấy Danh mục trang chủ",
            Application = "CLIENT WEB",
            Content = "Lấy danh sách category hiển thị trang chủ",
            Functions = "Trang chủ",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("featured-sale")]
    [HttpGet]
    public async Task<JsonResult> GetFeaturedOnSale()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _homeClient.GetFeaturedOnSale();

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER",
            IsCallApi = true,
            ApiName = "api/client/home/featured-sale",
            Actions = "Lấy Sản phẩm giảm giá",
            Application = "CLIENT WEB",
            Content = "Lấy danh sách sản phẩm Featured On Sale",
            Functions = "Trang chủ",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("seasonal-highlights")]
    [HttpGet]
    public async Task<JsonResult> GetSeasonalHighlights()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _homeClient.GetSeasonalHighlights();

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER",
            IsCallApi = true,
            ApiName = "api/client/home/seasonal-highlights",
            Actions = "Lấy Sản phẩm theo mùa",
            Application = "CLIENT WEB",
            Content = "Lấy danh sách sản phẩm Seasonal Highlights",
            Functions = "Trang chủ",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("customer-favorites")]
    [HttpGet]
    public async Task<JsonResult> GetCustomerFavorites()
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _homeClient.GetCustomerFavorites();

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER",
            IsCallApi = true,
            ApiName = "api/client/home/customer-favorites",
            Actions = "Lấy Sản phẩm yêu thích",
            Application = "CLIENT WEB",
            Content = "Lấy danh sách sản phẩm Customer Favorites",
            Functions = "Trang chủ",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
