using MediatR;
using SmartUniversity.Modules.Identity.Application.Commands;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Handlers;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        Guid userId = Guid.Parse(request.UserId);
        if (userId == Guid.Empty)
        {
            throw new InvalidUserException("User Id is required to deactivate user");
        }

        bool userExist = await _userRepository.ExistsByIdAsync(userId);
        if (!userExist)
        {
            throw new UserNotFoundException();
        }

        var user = await _userRepository.DeactivateUserAccount(userId);

        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
        };
    }
}