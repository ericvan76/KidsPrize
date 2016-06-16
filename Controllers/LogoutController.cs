using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KidsPrize.Controllers
{
    [Route("[controller]")]
    public class LogoutController : Controller
    {
        private readonly SignOutInteraction _signOutInteraction;

        public LogoutController(SignOutInteraction signOutInteraction)
        {
            _signOutInteraction = signOutInteraction;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.PrimaryAuthenticationType);
            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            return Redirect("~/");
        }
    }
}
