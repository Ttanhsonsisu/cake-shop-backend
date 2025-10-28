using cake_shop_back_end.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp;

[Route("api/test")]
[ApiController]
public class TestController(IEmailSender _emailSender) : ControllerBase
{
    [Route("ping")]
    [HttpGet]
    public async Task<JsonResult> Ping()
    {
        _emailSender.SendEmailAsync("nguythihai34@gmail.com", "123", "12123");

        return new JsonResult(new
        {
            Code = "200",
            Message = "Pong"
        })
        { StatusCode = 200 };
    }
}
