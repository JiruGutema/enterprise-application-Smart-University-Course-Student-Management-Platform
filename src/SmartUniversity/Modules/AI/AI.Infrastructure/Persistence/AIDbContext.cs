using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.AI.Domain.Entities;

namespace SmartUniversity.Modules.AI.Infrastructure.Persistence;

public class AIDbContext : DbContext
{
    public AIDbContext(DbContextOptions<AIDbContext> options)
        : base(options) { }

    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ai");

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.ToTable("chat_history");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.UserPrompt).IsRequired();
            entity.Property(c => c.AIResponse).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();
        });
    }
}
