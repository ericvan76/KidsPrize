using System;
using System.Collections.Generic;
using AutoMapper;
using KidsPrize.Bus;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Tests.Common
{
    public static class TestHelper
    {
        public static UserInfo CreateUser(KidsPrizeDbContext context)
        {
            var uid = Guid.NewGuid();
            var u = new User(0, uid, "test@user.com", "Test", "User", "TestUser", new HashSet<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) }, new List<Child>());
            context.Users.Add(u);
            context.SaveChanges();
            return new UserInfo() { Uid = uid };
        }

        public static KidsPrizeDbContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<KidsPrizeDbContext>();
            opts.UseInMemoryDatabase();
            return new KidsPrizeDbContext(opts.Options);
        }

        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(c => c.AddProfile(new Resources.MappingProfile())).CreateMapper();
        }
    }
}