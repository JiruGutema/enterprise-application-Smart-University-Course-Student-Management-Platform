using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Infrastructure.Persistence
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.ToTable("notifications", "notification");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Message).IsRequired().HasMaxLength(2000);

            builder.Property(x => x.Type).HasConversion<string>().IsRequired();

            builder.Property(x => x.IsRead).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.ReadAt);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsRead);
        }
    }

    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<Notifications> Notifications => Set<Notifications>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
        }
    }
}
