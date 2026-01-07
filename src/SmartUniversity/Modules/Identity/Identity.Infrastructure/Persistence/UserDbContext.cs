using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Infrastructure.Outbox;

namespace SmartUniversity.Modules.Identity.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // schema
        modelBuilder.HasDefaultSchema("identity");

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");

            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);

            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // Outbox
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type).IsRequired();

            entity.Property(x => x.Payload).IsRequired().HasColumnType("jsonb");

            entity.Property(x => x.OccurredAt).IsRequired();

            entity.Property(x => x.ProcessedAt);

            entity.Property(x => x.RetryCount).IsRequired();

            entity.Property(x => x.Error);
        });
    }
}
