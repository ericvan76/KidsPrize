using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using KidsPrize.Models;

namespace KidsPrize.Http.Extensions
{
    public static class UserExtensions
    {
        public static IEnumerable<Claim> Claims(this User user)
        {
            return new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Subject, user.Uid.ToString()),
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.Name, user.DisplayName),
                    new Claim(JwtClaimTypes.GivenName, user.GivenName),
                    new Claim(JwtClaimTypes.FamilyName, user.FamilyName),
                    new Claim(JwtClaimTypes.IdentityProvider, "Local"),
                    new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
                };
        }
    }
}