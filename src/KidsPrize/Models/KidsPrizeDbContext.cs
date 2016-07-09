using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Models
{
    public class KidsPrizeDbContext : DbContext
    {
        public KidsPrizeDbContext(DbContextOptions options):base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Day> Days { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasMany(u => u.Children).WithOne().IsRequired();
            modelBuilder.Entity<User>().HasMany(u => u.Identifiers).WithOne().IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.Uid).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Identifier>().HasIndex(i => new { i.Issuer, i.Value }).IsUnique();
            modelBuilder.Entity<Child>().HasIndex(c => c.Uid).IsUnique();
            modelBuilder.Entity<Day>().HasOne(d => d.Child).WithMany().IsRequired();
            modelBuilder.Entity<Day>().HasMany(d => d.Scores).WithOne().IsRequired();
            modelBuilder.Entity<Day>().HasAlternateKey("ChildId", "Date" );
            modelBuilder.Entity<Score>().HasAlternateKey("DayId", "Task");
        }
    }
}