using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Domain.Entities;

namespace SmartUniversity.Modules.Identity.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");

            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);

            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PasswordHash).IsRequired();
        });
    }
}
