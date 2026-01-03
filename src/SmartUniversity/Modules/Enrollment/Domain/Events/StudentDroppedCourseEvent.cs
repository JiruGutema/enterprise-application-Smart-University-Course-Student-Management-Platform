namespace SmartUniversity.Modules.Enrollment.Domain.Events;

public record StudentDroppedCourseEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
