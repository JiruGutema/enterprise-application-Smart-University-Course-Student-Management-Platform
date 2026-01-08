using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record CreateModuleCommand(
    Guid CourseId,
    string Name,
    string? Description,
    int Order
) : IRequest<Guid>;