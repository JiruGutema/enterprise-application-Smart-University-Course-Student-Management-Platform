namespace SmartUniversity.Modules.Courses.Domain.Events;

public sealed record CourseCreatedEvent(
    Guid CourseId,
    string Title,
    string Code,
    Guid InstructorId
);
