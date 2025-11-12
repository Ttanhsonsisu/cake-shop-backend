using System.Security.Claims;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Product;

[Route("api/cms/variant")]
[ApiController]
[Authorize(Policy = "WebAdminUser")]
public class VariantController(IVariant _variant, ILoggingHelpers _loggingHelpers) : ControllerBase
{
    [Route("create")]
    [HttpPost]
    public async Task<JsonResult> GetList(VariantRequest request)
    {
        var username = User.Claims.Where(p => p.Type.Equals(ClaimTypes.Name)).FirstOrDefault();

        APIResponse data = await _variant.CreateDraftAsync(request, username.Value);

        var remoteIP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _loggingHelpers.InsertLogging(new LoggingRequest
        {
            UserType = Consts.USER_TYPE_WEB_ADMIN,
            IsCallApi = true,
            ApiName = "api/cms/variant/create",
            Actions = "Tạo mới sản phẩm",
            Application = "WEB ADMIN",
            Content = "Tạo mới sản phẩm",
            Functions = "Danh mục",
            IsLogin = false,
            ResultLogging = data.Code == "200" ? "Thành công" : "Thất bại",
            UserCreated = username?.Value ?? "Unknown",
            IP = remoteIP
        });

        return new JsonResult(data) { StatusCode = 200 };
    }
}
