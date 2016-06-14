using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Models
{
    public class KidsPrizeDbContext : DbContext
    {
        public KidsPrizeDbContext(DbContextOptions<KidsPrizeDbContext> options):base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Day> Days { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}