using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Commands;

public record DeactivateUserCommand(string UserId) : IRequest<UserResponse>;