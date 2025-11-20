using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.SeasonalEvent;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.SeasonalEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.SeasonalEvent;

[Route("api/cms/seasonalEvent")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class SeasonalEventController(ISeasonalEvent _seasonalEvent, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(SeasonalEventRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _seasonalEvent.GetListAsync(request);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/seasonalEvent/list",
            Actions = "Danh sách sự kiện mùa",
            Application = "WEB ADMIN",
            Content = "Xem danh sách sự kiện theo mùa",
            Functions = "Sự kiện",
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
        APIResponse data = await _seasonalEvent.GetDetailAsync(id);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = $"api/cms/seasonalEvent/{id}",
            Actions = "Chi tiết sự kiện mùa",
            Application = "WEB ADMIN",
            Content = "Xem chi tiết sự kiện theo mùa",
            Functions = "Sự kiện",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> Create(SeasonalEventRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _seasonalEvent.CreateAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/seasonalEvent/create",
            Actions = "Tạo sự kiện mùa",
            Application = "WEB ADMIN",
            Content = "Tạo mới sự kiện theo mùa",
            Functions = "Sự kiện",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("update")]
    [HttpPost]
    public async Task<JsonResult> Update(SeasonalEventRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _seasonalEvent.UpdateAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/seasonalEvent/update",
            Actions = "Cập nhật sự kiện mùa",
            Application = "WEB ADMIN",
            Content = "Cập nhật sự kiện theo mùa",
            Functions = "Sự kiện",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus(SeasonalEventRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _seasonalEvent.ChangeStatusAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/seasonalEvent/changeStatus",
            Actions = "Đổi trạng thái sự kiện",
            Application = "WEB ADMIN",
            Content = "Bật/Tắt trạng thái sự kiện",
            Functions = "Sự kiện",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete(SeasonalEventRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _seasonalEvent.DeleteAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/seasonalEvent/delete",
            Actions = "Xóa sự kiện mùa",
            Application = "WEB ADMIN",
            Content = "Xóa sự kiện theo mùa",
            Functions = "Sự kiện",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
