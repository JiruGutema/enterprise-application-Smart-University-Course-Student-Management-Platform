using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record CreateCourseCommand(
    string Title,
    string Code,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid InstructorId,
    List<string>? Prerequisites
) : IRequest<Guid>;
