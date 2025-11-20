using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Marketing;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Marketing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Marketing;

[Route("api/cms/discountCode")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class DiscountCodeController(IDiscountCode _discountCode, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(DiscountCodeRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.GetListAsync(request);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCode/list",
            Actions = "Danh sách mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Xem danh sách mã giảm giá",
            Functions = "Mã giảm giá",
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
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.GetDetailAsync(id);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = $"api/cms/discountCode/{id}",
            Actions = "Chi tiết mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Xem chi tiết mã giảm giá",
            Functions = "Mã giảm giá",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> Create(DiscountCodeRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.CreateAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCode/create",
            Actions = "Tạo mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Tạo mới mã giảm giá",
            Functions = "Mã giảm giá",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("update")]
    [HttpPost]
    public async Task<JsonResult> Update(DiscountCodeRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.UpdateAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCode/update",
            Actions = "Cập nhật mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin mã giảm giá",
            Functions = "Mã giảm giá",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus(DiscountCodeRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.ChangeStatusAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCode/changeStatus",
            Actions = "Đổi trạng thái mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Bật/Tắt trạng thái mã giảm giá",
            Functions = "Mã giảm giá",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete(DiscountCodeRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCode.DeleteAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCode/delete",
            Actions = "Xóa mã giảm giá",
            Application = "WEB ADMIN",
            Content = "Xóa mã giảm giá",
            Functions = "Mã giảm giá",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
