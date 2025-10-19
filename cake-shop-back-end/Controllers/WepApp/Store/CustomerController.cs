using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp.Store;

[Route("api/customer")]
[ApiController]
public class CustomerController(ICustomer _customer, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("list")]
    [Authorize(Policy = "WebAdminUser")]
    public async Task<JsonResult> GetList(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();

        APIResponse data = await _customer.GetListAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/list",
            Actions = "Danh sách khách Hàng",
            Application = "WEB ADMIN",
            Content = "Danh sách khách Hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    
    [HttpGet("{id}")]
    public async Task<JsonResult> GetDetail(Guid id)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        APIResponse data = await _customer.GetDetailAsync(id);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/" + id,
            Actions = "Chi tiết thông tin khách hàng",
            Application = "WEB ADMIN",
            Content = "Chi tiết thông tin khách hàng",
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
    public async Task<JsonResult> Create(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        APIResponse data = await _customer.CreateAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/create",
            Actions = "Tạo mơi thông tin khách hàng",
            Application = "WEB ADMIN",
            Content = "Tạo mơi thông tin khách hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    public async Task<JsonResult> Update(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        APIResponse data = await _customer.UpdateAsync(request , username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/update",
            Actions = "Cập nhật thông tin khách hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin khách hàng",
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
    public async Task<JsonResult> Delete(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        APIResponse data = await _customer.DeleteAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/delete",
            Actions = "Xóa khách hàng",
            Application = "WEB ADMIN",
            Content = "Xóa khách hàng",
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
    public async Task<JsonResult> ChangeStatus(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();

        APIResponse data = await _customer.ChangeStatusAsync(request);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/changeStatus",
            Actions = "Cập nhật trạng thái khách hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật trạng thái khách hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("updateBilling")]
    [HttpPost]
    public async Task<JsonResult> UpdateUpdateBilling(CustomerRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _customer.UpdateBillingAddressAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/customer/updateBilling",
            Actions = "Cập nhật thông tin liên lạc khách hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin liên lạc khách hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    } 


}
