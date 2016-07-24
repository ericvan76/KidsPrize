using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace KidsPrize.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid UserId(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            if (claim != null)
            {
                 return Guid.Parse(claim.Value);
            }
            return Guid.Empty;
        }
    }
}