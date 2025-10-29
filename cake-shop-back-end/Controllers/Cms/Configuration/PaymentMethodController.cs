using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using cake_shop_back_end.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Configuration;

[Route("api/paymentMethod")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class PaymentMethodController(ILoggingHelpers _loggingHelpers, IPaymentMethod _paymentMethod) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList([FromBody] PaymentMethodRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.GetListAsync(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/list",
            Actions = "Danh sách cấu hình thanh toán",
            Application = "WEB ADMIN",
            Content = "Danh sách cấu hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }

    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> Create([FromBody] PaymentMethodRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.CreateAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/update",
            Actions = "Cập nhật cấu hình thanh toán",
            Application = "WEB ADMIN",
            Content = "Cập nhật cấu hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }


    [Route("update")]
    [HttpPost]
    public async Task<JsonResult> Update([FromBody] PaymentMethodRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.UpdateAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/update",
            Actions = "Cập nhật cấu hình thanh toán",
            Application = "WEB ADMIN",
            Content = "Cập nhật cấu hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete([FromBody] PaymentMethodRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.DeleteAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/delete",
            Actions = "Xóa cấu hình thanh toán",
            Application = "WEB ADMIN",
            Content = "Xóa cấu hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }

    [HttpGet("{id}")]
    public async Task<JsonResult> GetDetail(int id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/" + id,
            Actions = "chi tiết hình thanh toán",
            Application = "WEB ADMIN",
            Content = "chi tiết hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus([FromBody] PaymentMethodRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _paymentMethod.ChangeStatusAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/paymentMethod/changeStatus",
            Actions = "Cập nhật trạng thái cấu hình thanh toán",
            Application = "WEB ADMIN",
            Content = "Cập nhật trạng thái cấu hình thanh toán",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }
}
