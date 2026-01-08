using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.ValueObjects;
using System.Linq;

namespace SmartUniversity.Modules.Courses.Infrastructure.Persistence;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses", "courses");

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Code).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.InstructorId).IsRequired();
            entity.Property(c => c.IsPublished).HasDefaultValue(false);
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
    }
}
