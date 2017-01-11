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
        public DbSet<Score> Scores { get; set; }
        public DbSet<TaskGroup> TaskGroups { get; set; }
        public DbSet<Preference> Preferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp", "public");

            modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.HasDefaultSchema("KidsPrize");

            modelBuilder.Entity<Child>().HasIndex(c => c.UserId);

            modelBuilder.Entity<Score>().HasOne(s => s.Child).WithMany().IsRequired();
            modelBuilder.Entity<Score>().HasAlternateKey("ChildId", "Date", "Task");

            modelBuilder.Entity<TaskGroup>().HasOne(tg => tg.Child).WithMany().IsRequired();
            modelBuilder.Entity<TaskGroup>().HasAlternateKey("ChildId", "EffectiveDate");
            modelBuilder.Entity<TaskGroup>().HasMany(tg => tg.Tasks).WithOne().IsRequired();
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
