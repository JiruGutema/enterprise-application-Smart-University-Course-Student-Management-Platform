using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseUnpublishedEvent(
    Guid CourseId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
