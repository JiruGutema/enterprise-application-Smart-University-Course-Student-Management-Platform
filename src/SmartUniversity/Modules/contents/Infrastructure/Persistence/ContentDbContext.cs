using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Content.Domain.Entities;

namespace SmartUniversity.Modules.Content.Infrastructure.Persistence;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options) { }

    public DbSet<Material> Materials => Set<Material>();

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
    }
}
