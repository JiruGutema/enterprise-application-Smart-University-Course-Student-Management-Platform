using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Domain.Enums;

namespace SmartUniversity.Modules.Identity.Application.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponse> RegisterAsync(CreateUserRequest request);
        Task<UserResponse> DeactivateUserAccountAsync(DeactivateUserAccountRequest request);
        Task<UserResponse> ActivateUserAccountAsync(ActivateUserAccountRequest request);
        Task<(UserResponse user, string refreshToken, string accessToken)> LoginAsync(
            LoginRequest request
        );
        Task<(string newAccessToken, string newRefreshToken)> RefreshAccessTokenAsync(
            string refreshToken
        );
        Task<UserResponse> GetUserByIdAsync(Guid userId);
        Task<UserResponse> GetUserByEmailAsync(string email);
        Task<UserResponse> AdminCreateUser(AdminCreateUserRequest request);
        Task<SearchUserResponse> SearchUsersAsync(SearchUserRequest request);
        Task<UserResponse> UpdateUserAsync(UpdateUserProfile request, string userId);
        Task<UserResponse> UpdateUserRoleAsync(UpdateRoleRequest role, string userId);
    }
}
