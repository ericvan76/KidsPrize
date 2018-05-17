using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;

namespace KidsPrize.Http
{
    public class OverridedJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        public override ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            SecurityToken validated;
            bool emailVerified = false;
            var principal = base.ValidateToken(securityToken, validationParameters, out validated);
            if (principal.HasClaim(c => c.Type == JwtClaimTypes.Email) &&
                principal.HasClaim(c => c.Type == JwtClaimTypes.EmailVerified &&
                bool.TryParse(c.Value, out emailVerified) && emailVerified))
            {
                validatedToken = validated;
                return principal;
            }
            throw new Exception("Email is not verified.");
        }
    }
}