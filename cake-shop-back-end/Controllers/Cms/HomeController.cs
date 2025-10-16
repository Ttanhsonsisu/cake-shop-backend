using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Ok("Welcome to the Cake Shop CMS API!");
    }
}
