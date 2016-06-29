using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Services;
using Microsoft.AspNetCore.Routing;
using KidsPrize.Models;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace KidsPrize.Controllers
{
    // NOTE: This is to support a missing feature from IdentityServer3 but not in IdentityServer4
    [Route("connect/external")]
    [AllowAnonymous]
    public class ExternalAuthController : Controller
    {
        private readonly IUserService _loginService;
        private readonly IMessageStore<ConsentResponse> _consentResponseStore;

        public ExternalAuthController(IUserService loginService, SignInInteraction signInInteraction, IMessageStore<ConsentResponse> consentResponseStore)
        {
            this._loginService = loginService;
            this._consentResponseStore = consentResponseStore;
        }

        public class ExternalAuthRequest
        {
            [Required]
            public string client_id { get; set; }
            [Required]
            public string response_type { get; set; }
            [Required]
            [RegularExpression(@"[\w\s]+")]
            public string scope { get; set; }
            [Required]
            public string redirect_uri { get; set; }
            public string state { get; set; }
            [Required]
            [RegularExpression(@"^idp:(Google|Facebook)( tenant:\w+)?$")]
            public string acr_values { get; set; }

        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Authorize([FromQuery] ExternalAuthRequest request)
        {
            var scopes = request.scope.Split(' ');
            if (!scopes.Contains("openid"))
            {
                return BadRequest("scope must contain openid.");
            }
            var arc_values = request.acr_values.Split(' ');
            var idp = arc_values.FirstOrDefault(s => s.StartsWith("idp:"))?.Substring(4);
            return new ChallengeResult(idp, new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(this.Callback), new RouteValueDictionary(request)).ToString()
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Callback([FromQuery] ExternalAuthRequest authReq)
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

            var user = await _loginService.FindUserByIdentifier(issuer, userId, email);
            if (user != null)
            {
                await _loginService.UpdateUser(user, issuer, userId, claims);
            }
            else
            {
                user = await _loginService.RegisterUser(issuer, userId, email, claims);
            }

            await IssueCookie(user, "External");
            await HttpContext.Authentication.SignOutAsync("External");

            // Push consent response into ConsentStore
            var message = new IdentityServer4.Models.Message<ConsentResponse>(
                new ConsentResponse()
                {
                    ScopesConsented = new List<string> { "openid", "profile", "api1" }
                });

            var p = new Dictionary<string, string>();
            p.Add("client_id", authReq.client_id);
            p.Add("response_type", authReq.response_type);
            p.Add("scope", authReq.scope);
            p.Add("redirect_uri", authReq.redirect_uri);
            p.Add("state", authReq.state);
            // p.Add("acr_values", authReq.acr_values);
            message.AuthorizeRequestParameters = p;
            message.ResponseUrl = authReq.redirect_uri;

            await _consentResponseStore.WriteAsync(message);

            // Redirect to AuthorizeAfterConsent Endpoint
            var uri = new UriBuilder();
            uri.Host = HttpContext.Request.Host.Host;
            uri.Port = HttpContext.Request.Host.Port ?? 80;
            uri.Scheme = HttpContext.Request.Scheme;
            uri.Path = IdentityServer4.Constants.RoutePaths.Oidc.AuthorizeAfterConsent;
            uri.Query = $"id={message.Id}";

            var url = uri.ToString();
            return Redirect(url);
        }

        private async Task IssueCookie(User user, string authType)
        {
            var ci = new ClaimsIdentity(user.Claims, authType, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, cp);
        }
    }
}
