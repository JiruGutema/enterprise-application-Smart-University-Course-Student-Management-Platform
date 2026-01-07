using MediatR;
using SmartUniversity.Modules.Identity.Application.Commands;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Security;
using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Modules.Identity.Application.Handlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, (UserResponse user, string refreshToken, string accessToken)>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IEventBus _eventBus;

    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _eventBus = eventBus;
    }

    public async Task<(UserResponse user, string refreshToken, string accessToken)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        bool userExist = await _userRepository.ExistsByEmailAsync(request.Email);
        if (!userExist)
        {
            throw new UserNotFoundException("User with this email doesn't exists!");
        }

        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        
        bool match = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!match)
        {
            throw new UserNotFoundException("Wrong Credential is provided!");
        }

        string accessToken = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString(), TokenType.Access);
        string refreshToken = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString(), TokenType.Refresh);

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Role = user.Role.ToString(),
        };

        var userLoggedInEvent = new UserLoggedInEvent(user.Id, user.Email, user.FullName, "Addis Ababa, Ethiopia", DateTime.Now);
        await _eventBus.PublishAsync(userLoggedInEvent);

        return (userResponse, refreshToken, accessToken);
    }
}