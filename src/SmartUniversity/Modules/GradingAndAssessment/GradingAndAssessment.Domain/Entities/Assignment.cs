using SmartUniversity.Modules.GradingAndAssessment.Domain.Events;
using SmartUniversity.Modules.GradingAndAssessment.Domain.ValueObjects;
using SmartUniversity.Shared.Kernel;

namespace SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;

public class Assignment : AggregateRoot
{
    public Guid AssignmentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public AssignmentType Type { get; private set; }
    public DateTime? DueDate { get; private set; }
    public decimal MaxScore { get; private set; }
    public decimal Weight { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Assignment() { } // EF Core

    public Assignment(Guid courseId, string title, string? description, AssignmentType type, 
        DateTime? dueDate, decimal maxScore, decimal weight = 100)
    {
        AssignmentId = Guid.NewGuid();
        CourseId = courseId;
        Title = title;
        Description = description;
        Type = type;
        DueDate = dueDate;
        MaxScore = maxScore;
        Weight = weight;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AssignmentCreatedEvent(AssignmentId, CourseId, Title, DueDate));
    }

    public void Update(string title, string? description, DateTime? dueDate, decimal maxScore, decimal weight)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        MaxScore = maxScore;
        Weight = weight;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AssignmentUpdatedEvent(AssignmentId, Title, DueDate));
    }
}