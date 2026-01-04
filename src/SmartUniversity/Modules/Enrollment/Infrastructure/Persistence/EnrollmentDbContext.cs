using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Enrollment.Infrastructure.Outbox;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Persistence
{
    public class EnrollmentDbContext : DbContext
    {
        public DbSet<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment> Enrollments 
            => Set<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>();
        
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("enrollments");

            modelBuilder.Entity<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Ignore(x => x.DomainEvents);
                builder.Property(x => x.Status).HasConversion<string>();
            });

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("outbox_messages", "outbox");
                builder.HasKey(x => x.Id);
            });
        }
    }
}
