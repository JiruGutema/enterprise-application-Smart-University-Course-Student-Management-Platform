using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Queries;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Handlers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchUserResponse>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<SearchUserResponse> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.SearchUsersAsync(request.Query, request.Page, request.PageSize);

        var users = result.Items.Select(u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
        }).ToList();

        return new SearchUserResponse
        {
            Data = users,
            Total = result.TotalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}