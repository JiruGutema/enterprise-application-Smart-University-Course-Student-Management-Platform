namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseTitleUpdatedEvent(
    Guid CourseId,
    string NewTitle,
    string NewCode
);
