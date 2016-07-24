using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Home
{
    public class HomeController : Controller
    {
        [HttpGet("/")]

        public IActionResult Index()
        {
            return View();
        }
    }
}