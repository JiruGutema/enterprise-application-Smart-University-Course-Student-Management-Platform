using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResponse>;