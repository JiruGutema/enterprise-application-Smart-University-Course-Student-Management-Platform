using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseInstructorChangedEvent(
    Guid CourseId,
    Guid NewInstructorId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
