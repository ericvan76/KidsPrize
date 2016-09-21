using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace IdentityServer
{
    public class IdentityContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp", "public");
            modelBuilder.HasDefaultSchema("Identity");

            modelBuilder.Entity<IdentityUser<Guid>>().Property(i => i.Email).IsRequired();
            modelBuilder.Entity<IdentityUser<Guid>>().Property(i => i.NormalizedEmail).IsRequired();
            modelBuilder.Entity<IdentityUser<Guid>>().HasAlternateKey(i => i.NormalizedEmail);
        }
    }
}