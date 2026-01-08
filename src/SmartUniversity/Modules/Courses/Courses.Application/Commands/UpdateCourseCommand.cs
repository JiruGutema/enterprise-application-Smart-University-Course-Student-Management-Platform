using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public record UpdateCourseCommand(
    Guid CourseId,
    string? Title,
    string? Code,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? InstructorId,
    List<string>? Prerequisites
) : IRequest;
