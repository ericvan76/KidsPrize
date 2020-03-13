using KidsPrize.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KidsPrizeContext>
    {
        public KidsPrizeContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<KidsPrizeContext>();
            builder.UseNpgsql(
                "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=;",
                b => b.MigrationsHistoryTable("__MigrationHistory", "KidsPrize")
            );
            return new KidsPrizeContext(builder.Options);
        }
    }

    public class KidsPrizeContext : DbContext
    {
        public KidsPrizeContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Child> Children { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<TaskGroup> TaskGroups { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Redeem> Redeems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp").HasDefaultSchema("public");

            modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.HasDefaultSchema("KidsPrize");

            modelBuilder.Entity<Child>().HasIndex(c => c.UserId);

            modelBuilder.Entity<Score>().HasOne(s => s.Child).WithMany().IsRequired();
            modelBuilder.Entity<Score>().HasAlternateKey("ChildId", "Date", "Task");

            modelBuilder.Entity<TaskGroup>().HasOne(tg => tg.Child).WithMany().IsRequired();
            modelBuilder.Entity<TaskGroup>().HasAlternateKey("ChildId", "EffectiveDate");
            modelBuilder.Entity<TaskGroup>().HasMany(tg => tg.Tasks).WithOne().IsRequired();

            modelBuilder.Entity<Redeem>().HasOne(s => s.Child).WithMany().IsRequired();
            modelBuilder.Entity<Redeem>().HasIndex("ChildId", "Timestamp");
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                var typeName = entity.DisplayName();
                entity.SetTableName(typeName);
            }
        }
    }

    public static class HostingExtensions
    {
        public static IServiceCollection AddNpgsqlDbContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<KidsPrizeContext>(builder =>
            {
                builder.UseNpgsql(connectionString, options =>
                {
                    options.MigrationsHistoryTable("__MigrationHistory", "KidsPrize");
                });
            });
        }

        public static IApplicationBuilder UseNpgsqlDbContext(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<KidsPrizeContext>().Database.Migrate();
            }
            return app;
        }
    }
}