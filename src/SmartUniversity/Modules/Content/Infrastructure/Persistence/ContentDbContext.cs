// using Microsoft.EntityFrameworkCore;
// using SmartUniversity.Modules.Content.Domain.Aggregates;
// using SmartUniversity.Modules.Content.Infrastructure.Outbox;

// namespace SmartUniversity.Modules.Content.Infrastructure.Persistence;

// public class ContentDbContext : DbContext
// {
//     public ContentDbContext(DbContextOptions<ContentDbContext> options)
//         : base(options) { }

//     public DbSet<Material> Materials => Set<Material>();
//     public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<Material>(entity =>
//         {
//             entity.ToTable("materials", "content");
//             entity.HasKey(x => x.Id);

//             entity.Property(x => x.FileName).IsRequired();
//             entity.Property(x => x.FilePath).IsRequired();
//             entity.Property(x => x.FileType).IsRequired();
//         });

//         modelBuilder.Entity<OutboxMessage>(entity =>
//         {
//             entity.ToTable("outbox_messages", "content");
//             entity.HasKey(x => x.Id);
            
//             entity.Property(x => x.Type).IsRequired();
//             entity.Property(x => x.Content).IsRequired();
//         });
//     }
// }
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Content.Domain.Aggregates;
using SmartUniversity.Modules.Content.Infrastructure.Outbox;

namespace SmartUniversity.Modules.Content.Infrastructure.Persistence;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options) { }

    // ✅ Real change: made DbSets writable and initialized to null!
    public DbSet<Material> Materials { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("materials", "content");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileName).IsRequired();
            entity.Property(x => x.FilePath).IsRequired();
            entity.Property(x => x.FileType).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages", "content");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.Content).IsRequired();
        });
    }
}
