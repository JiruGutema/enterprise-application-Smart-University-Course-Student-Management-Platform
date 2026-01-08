using MediatR;
using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Queries;

public record SearchUsersQuery(
    string Query,
    int Page,
    int PageSize
) : IRequest<SearchUserResponse>;