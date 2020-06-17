using AutoMapper;
using KivalitaAPI.AuditModels;
using KivalitaAPI.Common;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace KivalitaAPI.Data
{
    public class KivalitaApiContext : DbContext
    {
        private readonly AuditFactory _auditFactory;
        public KivalitaApiContext(DbContextOptions<KivalitaApiContext> options, IMapper mapper)
            : base(options)
        {
            _auditFactory = new AuditFactory(mapper);
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserHistory> UserHistory { get; set; }

        public DbSet<Token> Token { get; set; }
        public DbSet<TokenHistory> TokenHistory { get; set; }

        public DbSet<Post> Post { get; set; }
        public DbSet<PostHistory> PostHistory { get; set; }

        public DbSet<Image> Image { get; set; }
        public DbSet<ImageHistory> ImageHistory { get; set; }

        public DbSet<Job> Job { get; set; }
        //public DbSet<Job> Job { get; set; }

        public DbSet<Leads> Leads { get; set; }
        //public DbSet<Leads> Leads { get; set; }

        public DbSet<WpRdStation> WpRdStation { get; set; }
        //public DbSet<WpRdStation> WpRdStation { get; set; }

        public DbSet<Company> Company { get; set; }
        //public DbSet<Company> Company { get; set; }

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
            var dataChanges = ChangeTracker.Entries().Where(a => a.State == EntityState.Modified || a.State == EntityState.Added || a.State == EntityState.Deleted);
            if (dataChanges.Any())
            {
                dataChanges.ToList().ForEach(data => {
                    var baseObject = data.Entity as IEntity;
                    var auditData = _auditFactory.GetAuditObject(baseObject, data.State, data.State == EntityState.Added ? baseObject.CreatedBy: baseObject.UpdatedBy);
                    this.Add(auditData);
                });

            }
            return base.SaveChanges();
        }
    }
}
