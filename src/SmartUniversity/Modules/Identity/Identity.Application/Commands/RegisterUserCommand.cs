using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Commands;

public record RegisterUserCommand(
    string Email,
    string FullName,
    string Password
) : IRequest<UserResponse>;