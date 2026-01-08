using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseTitleUpdatedEvent(
    Guid CourseId,
    string NewTitle,
    string NewCode
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
