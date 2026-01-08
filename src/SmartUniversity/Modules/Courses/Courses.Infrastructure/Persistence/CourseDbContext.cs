using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.ValueObjects;
using SmartUniversity.Modules.Courses.Domain.Enums;
using SmartUniversity.Modules.Courses.Infrastructure.Outbox;
using System.Linq;

namespace SmartUniversity.Modules.Courses.Infrastructure.Persistence;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses", "courses");

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Code).HasConversion(c => c.Value, v => CourseCode.Create(v)).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.Status).HasDefaultValue(CourseStatus.Draft);
            entity.Property(c => c.InstructorId).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.UpdatedAt).IsRequired();

            // Convert List<CourseCode> ↔ string
            entity.Property(c => c.Prerequisites)
                  .HasConversion(
                      v => string.Join(',', v.Select(x => x.Value)), // List<CourseCode> → string
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => CourseCode.Create(x))      // string → List<CourseCode>
                            .ToList()
                  );
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("modules", "courses");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Description).HasMaxLength(1000);
            entity.Property(m => m.Order).IsRequired();
            entity.HasOne<Course>().WithMany().HasForeignKey("CourseId");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("lessons", "courses");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Content).HasMaxLength(5000);
            entity.Property(l => l.Order).IsRequired();
            entity.HasOne<Module>().WithMany().HasForeignKey("ModuleId");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages", "courses");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).IsRequired().HasMaxLength(500);
            entity.Property(o => o.Data).IsRequired();
            entity.Property(o => o.OccurredAt).IsRequired();
            entity.Property(o => o.Error).HasMaxLength(1000);
        });
    }
}
