using MediatR;

namespace SmartUniversity.Modules.Courses.Application.Commands;

public sealed record UpdateModuleCommand(
    Guid ModuleId,
    string? Name,
    string? Description,
    int? Order
) : IRequest;