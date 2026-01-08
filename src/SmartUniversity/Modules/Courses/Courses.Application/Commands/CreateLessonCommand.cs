using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record CreateLessonCommand(
    Guid ModuleId,
    string Name,
    string? Content,
    int Order
) : IRequest<Guid>;