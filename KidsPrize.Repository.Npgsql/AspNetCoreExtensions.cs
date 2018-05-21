using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Repository.Npgsql
{
    public static class AspNetCoreExtensions
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
            using(var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<KidsPrizeContext>().Database.Migrate();
            }
            return app;
        }
    }
}