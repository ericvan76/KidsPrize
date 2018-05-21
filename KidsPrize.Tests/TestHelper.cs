using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using KidsPrize.Repository.Npgsql;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Tests
{
    public static class TestHelper
    {
        public static KidsPrizeContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<KidsPrizeContext>();
            opts.UseInMemoryDatabase("test");
            return new KidsPrizeContext(opts.Options);
        }

        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(c => c.AddProfile(new MappingProfile())).CreateMapper();
        }

    }
}