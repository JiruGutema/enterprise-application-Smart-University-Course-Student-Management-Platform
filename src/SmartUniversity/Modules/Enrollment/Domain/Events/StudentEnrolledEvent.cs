using SmartUniversity.Modules.Enrollment.Domain.Enums;

namespace SmartUniversity.Modules.Enrollment.Domain.Events;

public record StudentEnrolledEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
