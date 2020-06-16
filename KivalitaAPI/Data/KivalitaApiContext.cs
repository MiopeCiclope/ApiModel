using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace KivalitaAPI.Data
{
    public class KivalitaApiContext : DbContext
    {
        public KivalitaApiContext(DbContextOptions<KivalitaApiContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Token> Token { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Leads> Leads { get; set; }
        public DbSet<WpRdStation> WpRdStation { get; set; }
        public DbSet<Company> Company { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(a => a.PostImage)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Job>()
                .HasOne(a => a.JobImage)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Leads>()
                .HasOne(l => l.Company)
                .WithMany(c => c.Leads)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(l => l.User)
                .WithMany(c => c.Company)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            var updateList = ChangeTracker.Entries().Where(a => a.State == EntityState.Modified).ToList();

            var result = base.SaveChanges();
            return result;
        }
    }
}
