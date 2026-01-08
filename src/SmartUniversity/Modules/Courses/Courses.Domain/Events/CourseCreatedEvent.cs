using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Courses.Domain.Events;

public sealed record CourseCreatedEvent(
    Guid CourseId,
    string Title,
    string Code,
    Guid InstructorId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
