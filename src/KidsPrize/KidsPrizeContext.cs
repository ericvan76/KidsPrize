using System;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace KidsPrize
{
    public class KidsPrizeContext : DbContext
    {
        public KidsPrizeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Child> Children { get; set; }
        public DbSet<Day> Days { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.HasPostgresExtension("uuid-ossp", "public");
            modelBuilder.HasDefaultSchema("KidsPrize");

            modelBuilder.Entity<Child>().HasIndex(c => c.UserId);
            modelBuilder.Entity<Day>().HasOne(d => d.Child).WithMany().IsRequired();
            modelBuilder.Entity<Day>().HasMany(d => d.Scores).WithOne().IsRequired();
            modelBuilder.Entity<Day>().HasAlternateKey("ChildId", "Date");
            modelBuilder.Entity<Score>().HasAlternateKey("DayId", "Task");
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                var typeName = entity.DisplayName();
                entity.Relational().TableName = typeName;
            }
        }
    }
}
