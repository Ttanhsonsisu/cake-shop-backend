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

[Route("api/cms/discountCampaign")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class CampaintController(ICampaign _discountCampaign, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [HttpPost]
    public async Task<JsonResult> GetList(CampaignRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCampaign.GetListAsync(request);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCampaign/list",
            Actions = "Danh sách khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Xem danh sách chương trình khuyến mãi",
            Functions = "Khuyến mãi",
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
        APIResponse data = await _discountCampaign.GetDetailAsync(id);
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = $"api/cms/discountCampaign/{id}",
            Actions = "Chi tiết khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Xem chi tiết khuyến mãi",
            Functions = "Khuyến mãi",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> Create(CampaignRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCampaign.CreateAsync(request, username?.Value ?? "Unknown");

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCampaign/create",
            Actions = "Tạo khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Tạo mới chương trình khuyến mãi",
            Functions = "Khuyến mãi",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("update")]
    [HttpPost]
    public async Task<JsonResult> Update(CampaignRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCampaign.UpdateAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCampaign/update",
            Actions = "Cập nhật khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin khuyến mãi",
            Functions = "Khuyến mãi",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changeStatus")]
    [HttpPost]
    public async Task<JsonResult> ChangeStatus(CampaignRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCampaign.ChangeStatusAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCampaign/changeStatus",
            Actions = "Đổi trạng thái khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Bật/Tắt trạng thái khuyến mãi",
            Functions = "Khuyến mãi",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("delete")]
    [HttpPost]
    public async Task<JsonResult> Delete(CampaignRequest request)
    {
        var username = User.Claims.FirstOrDefault(p => p.Type.Equals(ClaimTypes.Name));
        APIResponse data = await _discountCampaign.DeleteAsync(request, username?.Value ?? "Unknown");
        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/discountCampaign/delete",
            Actions = "Xóa khuyến mãi",
            Application = "WEB ADMIN",
            Content = "Xóa chương trình khuyến mãi",
            Functions = "Khuyến mãi",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
