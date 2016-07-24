using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize
{
    public class KidsPrizeContext : DbContext
    {
        public KidsPrizeContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<Child> Children { get; set; }
        public DbSet<Day> Days { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Child>().HasIndex(c => c.UserId);
            modelBuilder.Entity<Day>().HasOne(d => d.Child).WithMany().IsRequired();
            modelBuilder.Entity<Day>().HasMany(d => d.Scores).WithOne().IsRequired();
            modelBuilder.Entity<Day>().HasAlternateKey("ChildId", "Date" );
            modelBuilder.Entity<Score>().HasAlternateKey("DayId", "Task");
        }
    }
}