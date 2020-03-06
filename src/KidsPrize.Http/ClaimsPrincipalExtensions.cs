using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace KidsPrize
{
    public static class ClaimsPrincipalExtensions
    {
        public static string UserId(this ClaimsPrincipal principal)
        {
            // Use Email as the UserId, as Auth0's free plan doesn't support account linking
            var claim = principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);
            if (claim != null)
            {
                return claim.Value.ToLower();
            }
            return string.Empty;
        }
    }
}