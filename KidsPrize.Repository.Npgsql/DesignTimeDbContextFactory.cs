using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KidsPrize.Repository.Npgsql
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
}