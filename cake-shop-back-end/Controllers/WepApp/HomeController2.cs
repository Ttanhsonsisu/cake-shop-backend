using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.WepApp;

public class HomeController2 : ControllerBase
{
    public IActionResult Index()
    {
        return Ok();
    }
}
