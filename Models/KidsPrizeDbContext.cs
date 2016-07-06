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

            modelBuilder.Entity<User>()
                .HasMany(u=>u.Children)
                .WithOne()
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasMany(u => u.Identifiers)
                .WithOne()
                .IsRequired();
            modelBuilder.Entity<Day>()
                .HasOne(d => d.Child)
                .WithMany();
            modelBuilder.Entity<Day>()
                .HasMany(d=>d.Scores)
                .WithOne()
                .IsRequired();
        }
    }
}