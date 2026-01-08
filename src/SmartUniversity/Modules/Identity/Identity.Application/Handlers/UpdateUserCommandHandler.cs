using MediatR;
using SmartUniversity.Modules.Identity.Application.Commands;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Handlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        Guid id = Guid.Parse(request.UserId);
        string? passwordHash = null;
        
        if (request.Password != null)
        {
            passwordHash = _passwordHasher.Hash(request.Password);
        }

        bool exist = await _userRepository.ExistsByIdAsync(id);
        if (!exist)
        {
            throw new UserNotFoundException();
        }

        var user = await _userRepository.UpdateUserAsync(request.Email, request.FullName, passwordHash, id);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Role = user.Role.ToString(),
        };
    }
}