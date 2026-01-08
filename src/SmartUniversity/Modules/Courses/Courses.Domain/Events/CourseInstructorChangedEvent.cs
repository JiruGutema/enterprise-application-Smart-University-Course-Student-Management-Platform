namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseInstructorChangedEvent(
    Guid CourseId,
    Guid NewInstructorId
);
