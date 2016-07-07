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
            modelBuilder.Entity<Day>().HasOne(d => d.Child).WithMany().HasForeignKey(d => d.ChildId).IsRequired();
            modelBuilder.Entity<Day>().HasMany(d => d.Scores).WithOne().HasForeignKey(s => s.DayId).IsRequired();

            modelBuilder.Entity<User>().HasIndex(u => u.Uid).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Identifier>().HasIndex(i => new { i.Issuer, i.Value }).IsUnique();
            modelBuilder.Entity<Child>().HasIndex(c => c.Uid).IsUnique();
            modelBuilder.Entity<Day>().HasIndex(d => new { d.ChildId, d.Date }).IsUnique();
            modelBuilder.Entity<Score>().HasIndex(s => new { s.DayId, s.Task }).IsUnique();
        }
    }
}