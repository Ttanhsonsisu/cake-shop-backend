using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Configuration;

[Route("api/deliverySlot")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class DeliveryTimeSloteController(ILoggingHelpers _loggingHelpers, IDeliverySlot _deliverySlot) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList([FromBody] DeliverySlotRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _deliverySlot.GetListAsync(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/list",
            Actions = "Danh sách cấu hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "Danh sách cấu hình vận chuyển",
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
    public async Task<JsonResult> Create([FromBody] DeliverySlotRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _deliverySlot.CreateAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/update",
            Actions = "Cập nhật cấu hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "Cập nhật cấu hình vận chuyển",
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
    public async Task<JsonResult> Update([FromBody] DeliverySlotRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _deliverySlot.UpdateAsync(req, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/update",
            Actions = "Cập nhật cấu hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "Cập nhật cấu hình vận chuyển",
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
    public async Task<JsonResult> Delete([FromBody] DeliverySlotRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _deliverySlot.DeleteAsync(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/delete",
            Actions = "Xóa cấu hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "Xóa cấu hình vận chuyển",
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

        var response = await _deliverySlot.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/" + id,
            Actions = "chi tiết hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "chi tiết hình vận chuyển",
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
    public async Task<JsonResult> ChangeStatus([FromBody] DeliverySlotRequest req)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        var response = await _deliverySlot.ChangeStatusAsync(req);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/deliverySlot/changeStatus",
            Actions = "Cập nhật trạng thái cấu hình vận chuyển",
            Application = "WEB ADMIN",
            Content = "Cập nhật trạng thái cấu hình vận chuyển",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = response.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP.ToString()
        });

        return new JsonResult(response) { StatusCode = 200 };
    }
}
