using System;
using System.Security.Claims;
using AutoMapper;
using IdentityModel;
using KidsPrize.Bus;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Tests.Common
{
    public static class TestHelper
    {
        public static ClaimsPrincipal CreateUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimTypes.Subject, Guid.NewGuid().ToString())
            }));
        }

        public static void SetAuthorisation(this Command command, ClaimsPrincipal user)
        {
            command.SetHeader("Authorisation", user);
        }

        public static KidsPrizeContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<KidsPrizeContext>();
            opts.UseInMemoryDatabase();
            return new KidsPrizeContext(opts.Options);
        }

        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(c => c.AddProfile(new Resources.MappingProfile())).CreateMapper();
        }
    }
}