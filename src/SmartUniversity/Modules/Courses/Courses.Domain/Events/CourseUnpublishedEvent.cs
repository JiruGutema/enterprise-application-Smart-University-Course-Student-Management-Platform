namespace SmartUniversity.Modules.Courses.Domain.Events;

public record CourseUnpublishedEvent(
    Guid CourseId
);
