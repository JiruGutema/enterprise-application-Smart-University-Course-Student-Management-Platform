using SmartUniversity.Modules.Identity.Application.DTO;

namespace SmartUniversity.Modules.Identity.Application.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponse> RegisterAsync(CreateUserRequest request);
        Task<UserResponse> DeactivateUserAccountAsync(DeactivateUserAccountRequest request);
  Task<(UserResponse user, string refreshToken, string accessToken)> LoginAsync(LoginRequest request);

    }
}
