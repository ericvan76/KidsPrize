using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using KidsPrize.Bus;
using KidsPrize.Models;

namespace KidsPrize.Http.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static UserInfo GetUserInfo(this ClaimsPrincipal principal)
        {
            var userInfo = new UserInfo();
            var claim = principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            if (claim != null)
            {
                userInfo.Uid = Guid.Parse(claim.Value);
            }
            return userInfo;
        }

        public static User CreateUser(this ClaimsPrincipal principal)
        {
            var claims = principal.Claims;
            var userIdClaim = claims.FirstOrDefault(i => i.Type == JwtClaimTypes.Subject)
                ?? claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userId");
            }
            var emailClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new Exception("Unknown email");
            }

            var givenNameClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.GivenName);
            var familyNameClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.Surname);
            var displayNameClaim = claims.FirstOrDefault(i => i.Type == ClaimTypes.Name);

            return new User(
                0,
                Guid.NewGuid(),
                emailClaim.Value,
                givenNameClaim?.Value ?? string.Empty,
                familyNameClaim?.Value ?? string.Empty,
                displayNameClaim?.Value ?? emailClaim.Value.Split('@')[0],
                new List<Identifier>() { new Identifier(0, userIdClaim.Issuer, userIdClaim.Value) },
                new List<Child>()
                );

        }
    }
}