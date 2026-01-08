using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record UpdateLessonCommand(
    Guid LessonId,
    string? Name,
    string? Content,
    int? Order
) : IRequest;