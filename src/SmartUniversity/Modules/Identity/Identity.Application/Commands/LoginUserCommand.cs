using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Commands;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<(UserResponse user, string refreshToken, string accessToken)>;