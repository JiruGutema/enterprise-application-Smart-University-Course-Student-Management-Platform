using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.GradingAndAssessment.Domain.Events;

public record AssignmentCreatedEvent(
    Guid AssignmentId,
    Guid CourseId,
    string Title,
    DateTime? DueDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record AssignmentUpdatedEvent(
    Guid AssignmentId,
    string Title,
    DateTime? DueDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record GradeRecordedEvent(
    Guid GradeId,
    Guid EnrollmentId,
    Guid AssignmentId,
    decimal Score
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record GradeUpdatedEvent(
    Guid GradeId,
    decimal NewScore,
    string? Feedback
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}