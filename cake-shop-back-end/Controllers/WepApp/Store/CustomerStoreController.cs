using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp.Store;

[Route("api/store")]
[Authorize(Policy = "WebStoreUser")]
[ApiController]
public class CustomerStoreController(ICustomer _customer, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    // get infor of customer by customer token
    [HttpGet("customerInfo")]
    public async Task<JsonResult> GetCustomerInfo()
    {
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        var idUser = customerToken != null ? Guid.Parse(customerToken.Value) : Guid.Empty;
        APIResponse data = await _customer.GetInfoCustomerWithIdUser(idUser);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new DataObjects.Requests.Common.LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/store/customer-info",
            Actions = "Thông tin khách Hàng",
            Application = "WEB ADMIN",
            Content = "Thông tin khách Hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = customerToken?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [HttpGet("accountSetting")]
    public async Task<JsonResult> GetAccountSetting()
    {
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        var idUser = customerToken != null ? Guid.Parse(customerToken.Value) : Guid.Empty;

        APIResponse data = await _customer.GetInforSettingAccountCustomer(idUser);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new DataObjects.Requests.Common.LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/store/accountSetting",
            Actions = "Thông tin tài khoản khách Hàng",
            Application = "WEB ADMIN",
            Content = "Thông tin tài khoản khách Hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = customerToken?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("updateSettingAccount")]
    [HttpPost]
    public async Task<JsonResult> updateSettingAccount(CustomerRequest request)
    {
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var idUser = customerToken != null ? Guid.Parse(customerToken.Value) : Guid.Empty;

        APIResponse data = await _customer.UpdateAccountSettingAsync(idUser, request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new DataObjects.Requests.Common.LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/store/updateSettingAccount",
            Actions = "Cập nhật thông tin tài khoản khách Hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin tài khoản khách Hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = customerToken?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("updateBilling")]
    [HttpPost]
    public async Task<JsonResult> UpdateBilling(CustomerRequest request)
    {
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var idUser = customerToken != null ? Guid.Parse(customerToken.Value) : Guid.Empty;

        APIResponse data = await _customer.UpdateBillingAddressAsync(idUser, request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new DataObjects.Requests.Common.LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/store/updateBilling",
            Actions = "Cập nhật thông tin liên hệ khách Hàng",
            Application = "WEB ADMIN",
            Content = "Cập nhật thông tin liên hệ khách Hàng",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = customerToken?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

    [Route("changePassword")]
    [HttpPost]
    public async Task<JsonResult> UpdateBilchangePasswordling(PasswordRequest request)
    {
        var customerToken = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Surname)).FirstOrDefault();
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();
        var idUser = customerToken != null ? Guid.Parse(customerToken.Value) : Guid.Empty;

        APIResponse data = await _customer.ChangePassword(idUser, request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _loggingHelpers.InsertLogging(new DataObjects.Requests.Common.LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/store/changePassword",
            Actions = "đổi mật khẩu ",
            Application = "WEB ADMIN",
            Content = "đổi mật khẩu ",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = customerToken?.Value ?? "Unknown",
            IP = remoteIP
        });
        return new JsonResult(data) { StatusCode = 200 };
    }

}
