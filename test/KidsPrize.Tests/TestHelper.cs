using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using IdentityModel;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Tests
{
    public static class TestHelper
    {
        public static ClaimsPrincipal CreateUser(KidsPrizeContext context)
        {
            var userId = Guid.NewGuid();
            context.Preferences.Add(new Preference(userId, (int)DateTimeOffset.Now.Offset.TotalMinutes));
            context.SaveChanges();
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimTypes.Subject, userId.ToString())
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
            return new MapperConfiguration(c => c.AddProfile(new MappingProfile())).CreateMapper();
        }

        public static void ValidateModel(Command command)
        {
            var context = new ValidationContext(command, null, null);
	        var results = new List<ValidationResult>();
	        Validator.ValidateObject(command, context);
        }
    }
}