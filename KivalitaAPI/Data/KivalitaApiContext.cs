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
        public DbSet<JobHistory> JobHistory { get; set; }

        public DbSet<Leads> Leads { get; set; }
        public DbSet<LeadsHistory> LeadsHistory { get; set; }

        public DbSet<Company> Company { get; set; }
        public DbSet<CompanyHistory> CompanyHistory { get; set; }

        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryHistory> CategoryHistory { get; set; }

        public DbSet<Template> Template { get; set; }
        public DbSet<TemplateHistory> TemplateHistory { get; set; }

        public DbSet<WpRdStation> WpRdStation { get; set; }
        public DbSet<MicrosoftToken> MicrosoftToken { get; set; }

        public DbSet<Flow> Flow { get; set; }
        public DbSet<FlowHistory> FlowHistory { get; set; }

        public DbSet<FlowAction> FlowAction { get; set; }
        public DbSet<FlowActionHistory> FlowActionHistory { get; set; }

        public DbSet<FlowTask> FlowTask { get; set; }
        public DbSet<FlowTaskHistory> FlowTaskHistory { get; set; }

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

            modelBuilder.Entity<Category>()
                .HasData(
                    new Category { Id = 1, Name = "Administração" },
                    new Category { Id = 2, Name = "Financeiro" },
                    new Category { Id = 3, Name = "Marketing" },
                    new Category { Id = 4, Name = "RH" }
                );

            modelBuilder.Entity<Template>()
                .HasOne(t => t.Category)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flow>()
                .HasOne(t => t.Filter)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flow>()
                .HasMany(f => f.FlowAction)
                .WithOne(f => f.Flow)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlowTask>()
                .HasOne(f => f.Leads)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FlowTask>()
                .HasOne(f => f.FlowAction)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
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
                    if(auditData != null)
                    {
                        auditData.TableId = baseObject.Id;
                        this.Add(auditData);
                    }
                });
            }
            return base.SaveChanges();
        }
    }
}
