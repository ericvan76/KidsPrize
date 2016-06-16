using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Services;
using Microsoft.AspNetCore.Routing;
using KidsPrize.Models;

namespace KidsPrize.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IUserService loginService;
        public LoginController(IUserService loginService)
        {
            this.loginService = loginService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult External([FromQuery]string provider, [FromQuery]string signInId)
        {
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(this.ExternalCallback), new RouteValueDictionary { { nameof(signInId), signInId } }).ToString()
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ExternalCallback([FromQuery]string signInId)
        {
            var claimPrincipal = await HttpContext.Authentication.AuthenticateAsync("External");
            if (claimPrincipal == null)
            {
                throw new Exception();
            }

            var claims = claimPrincipal.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(i => i.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userId");
            }

            var emailClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new Exception("Unknown email");
            }

            claims.Remove(userIdClaim);

            var issuer = userIdClaim.Issuer;
            var userId = userIdClaim.Value;
            var email = emailClaim.Value;

            var user = await loginService.FindUserByIdentifier(issuer, userId, email);
            if (user != null)
            {
                await loginService.UpdateUser(user, issuer, userId, claims);
            }
            else
            {
                user = await loginService.RegisterUser(issuer, userId, email, claims);
            }

            await IssueCookie(user, "External");
            await HttpContext.Authentication.SignOutAsync("External");

            return Redirect("~/");
        }

        private async Task IssueCookie(User user, string authType)
        {
            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, user.Uid.ToString()),
                new Claim(JwtClaimTypes.Name, user.DisplayName),
                new Claim(JwtClaimTypes.IdentityProvider, "Local"),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
            };
            var ci = new ClaimsIdentity(claims, authType, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, cp);
        }
    }
}
