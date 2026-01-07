using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Queries;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppException("User id is required");
        }

        bool userExist = await _userRepository.ExistsByIdAsync(request.UserId);
        if (!userExist)
        {
            throw new UserNotFoundException();
        }

        var user = await _userRepository.GetUserByIdAsync(request.UserId);

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