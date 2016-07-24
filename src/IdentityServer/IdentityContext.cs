using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityServer
{
    public class IdentityContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<IdentityUser<Guid>>().Property(i => i.Email).IsRequired();
            modelBuilder.Entity<IdentityUser<Guid>>().Property(i => i.NormalizedEmail).IsRequired();
            modelBuilder.Entity<IdentityUser<Guid>>().HasAlternateKey(i => i.NormalizedEmail);
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                var typeName = entity.DisplayName();
                var regex = new Regex(@"\<\w+\>");
                var tableName = regex.Replace(typeName, string.Empty);
                entity.Relational().TableName = tableName;
            }
        }
    }
}