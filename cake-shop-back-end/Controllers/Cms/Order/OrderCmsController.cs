using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Order;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Order;

[Route("api/cms/order")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class OrderCmsController(IOrder _order, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("createDraft")]
    [HttpPost]
    public async Task<JsonResult> CreateOrderDraft(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.CreateOrderDrafFromAdminAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/create-draft",
            Actions = "Tạo nháp đơn hàng từ Admin",
            Application = "WEB ADMIN",
            Content = "Tạo nháp đơn hàng từ Admin",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.GetOrderListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/list",
            Actions = "Danh sách đơn hàng",
            Application = "WEB ADMIN",
            Content = "Xem danh sách đơn hàng",
            Functions = "Quản lý đơn hàng",
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

        APIResponse data = await _order.DetailOrderNomalAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/" + id,
            Actions = "Chi tiết đơn hàng",
            Application = "WEB ADMIN",
            Content = "Xem chi tiết đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.DeleteOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/delete",
            Actions = "Xóa đơn hàng",
            Application = "WEB ADMIN",
            Content = "Xóa đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("publish")]
    [HttpPost]
    public async Task<JsonResult> PublishFromAdmin(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.PublishFromAdminOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/publish",
            Actions = "Publish đơn hàng từ Admin",
            Application = "WEB ADMIN",
            Content = "Publish đơn hàng từ Admin",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("updateStatus")]
    [HttpPost]
    public async Task<JsonResult> UpdateStatus(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.UpdateStatusOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/update-status",
            Actions = "Cập nhật trạng thái đơn hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật trạng thái đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("confirmPayment")]
    [HttpPost]
    public async Task<JsonResult> ConfirmPayment(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.ComfirmPaymentAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/confirm-payment",
            Actions = "Xác nhận thanh toán",
            Application = "WEB ADMIN",
            Content = "Xác nhận thanh toán đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("confirmOrder")]
    [HttpPost]
    public async Task<JsonResult> ConfirmOrder(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.ComfirmOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/confirm-order",
            Actions = "Xác nhận đơn hàng",
            Application = "WEB ADMIN",
            Content = "Xác nhận đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("cancel")]
    [HttpPost]
    public async Task<JsonResult> CancelOrder(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.CancelOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/cancel",
            Actions = "Hủy đơn hàng",
            Application = "WEB ADMIN",
            Content = "Hủy đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus(OrderRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _order.ChangeStatusOrderAsync(request, username?.Value ?? "");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/order/change-status",
            Actions = "Thay đổi trạng thái",
            Application = "WEB ADMIN",
            Content = "Thay đổi trạng thái đơn hàng",
            Functions = "Quản lý đơn hàng",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
