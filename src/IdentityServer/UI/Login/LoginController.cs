using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;

namespace IdentityServer.UI.Login
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IUserInteractionService _interaction;

        public LoginController(
            ILoginService loginService,
            IUserInteractionService interaction)
        {
            _loginService = loginService;
            _interaction = interaction;
        }

        [HttpGet("ui/login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel();

            if (returnUrl != null)
            {
                var context = await _interaction.GetLoginContextAsync();
                if (context != null)
                {
                    vm.Username = context.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        [HttpPost("ui/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _loginService.ValidateCredentials(model.Username, model.Password))
                {
                    var user = await _loginService.FindByUsername(model.Username);
                    await IssueCookie(user, "idsvr", "password");

                    if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }

        private async Task IssueCookie(
            IdentityUser<Guid> user,
            string idp,
            string amr)
        {
            var name = user.Claims.Where(x => x.ClaimType == JwtClaimTypes.Name).Select(x => x.ClaimValue).FirstOrDefault() ?? "Unknown";

            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, idp),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
            };
            var ci = new ClaimsIdentity(claims, amr, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.DefaultCookieAuthenticationScheme, cp);
        }

        [HttpGet("/ui/external/{provider}", Name = "External")]
        public IActionResult External([FromRoute] string provider, [FromQuery] string returnUrl)
        {
            var parametersToAdd = new Dictionary<string, string>();
            if (returnUrl != null)
            {
                parametersToAdd["returnUrl"] = returnUrl;
            }
            var redirectUri = QueryHelpers.AddQueryString("/ui/external-callback", parametersToAdd);
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }

        [HttpGet("/ui/external-callback")]
        public async Task<IActionResult> ExternalCallback([FromQuery] string returnUrl)
        {
            var tempUser = await HttpContext.Authentication.AuthenticateAsync("Temp");
            if (tempUser == null)
            {
                throw new Exception();
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject || x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new ArgumentException("Could not find UserId from external claims.");
            }
            claims.Remove(userIdClaim);

            var emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email || x.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new ArgumentException("Could not find Email from external claims.");
            }

            var provider = userIdClaim.Issuer;
            var userId = userIdClaim.Value;
            var email = emailClaim.Value;

            // AutoProvision or Update User
            var user = await _loginService.AutoProvisionUser(provider, userId, email, claims);

            await IssueCookie(user, provider, "external");
            await HttpContext.Authentication.SignOutAsync("Temp");

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
                // todo: signin
                //return new SignInResult(signInId);
            }

            return Redirect("~/");

        }
    }
}
