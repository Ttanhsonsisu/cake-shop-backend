using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Store;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp.Store;

[Route("api/client/products")]
[ApiController]
[AllowAnonymous]
public class CakeShopStoreController(ICakeShopStore _productClient, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [HttpPost("list")] // Dùng POST để gửi body JSON phức tạp dễ hơn GET
    public async Task<JsonResult> GetList([FromBody] CakeShopRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));

        var data = await _productClient.GetListProducts(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        // Log search query nếu có để phân tích hành vi người dùng
        string logContent = "Xem danh sách sản phẩm";
        if (!string.IsNullOrEmpty(request.SearchQuery))
            logContent += $" - Search: {request.SearchQuery}";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = "CUSTOMER",
            IsCallApi = true,
            ApiName = "api/client/products/list",
            Actions = "Listing Product",
            Application = "CLIENT WEB",
            Content = logContent,
            Functions = "Product Page",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Guest",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
