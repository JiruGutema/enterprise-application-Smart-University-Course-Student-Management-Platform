using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record DeleteModuleCommand(
    Guid ModuleId
) : IRequest;