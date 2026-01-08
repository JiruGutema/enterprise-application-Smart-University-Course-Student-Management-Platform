using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record DeleteLessonCommand(
    Guid LessonId
) : IRequest;