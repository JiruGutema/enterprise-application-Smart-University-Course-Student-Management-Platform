using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;

namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

public class GradingDbContext : DbContext
{
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public GradingDbContext(DbContextOptions<GradingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("assessments");

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Type).HasColumnName("type").HasConversion<string>();
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.MaxScore).HasColumnName("max_score").HasPrecision(6, 2);
            entity.Property(e => e.Weight).HasColumnName("weight").HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("assignments");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId);
            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.Score).HasColumnName("score").HasPrecision(6, 2);
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.GradedByInstructorId).HasColumnName("graded_by_instructor_id");
            entity.Property(e => e.GradedAt).HasColumnName("graded_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("grades");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.Data).HasColumnType("jsonb");
            entity.ToTable("outbox_messages");
        });
    }
}