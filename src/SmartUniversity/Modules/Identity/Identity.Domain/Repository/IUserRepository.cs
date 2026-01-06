using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Shared.Pagination;

namespace SmartUniversity.Modules.Identity.Domain.Repository
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByIdAsync(Guid Id);
        Task<User> DeactivateUserAccount(Guid Id);
        Task<User> ActivateUserAccount(Guid Id);
        Task<User?> GetUserByIdAsync(Guid Id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<PagedResult<User>> SearchUsersAsync(string query, int page, int pageSize);
        Task AddAsync(User user);
        Task<User> UpdateUserAsync(string? email, string? fullName, string? passwordHash, Guid id);
        Task<User> UpdateUserRoleAsync(Role Role, Guid id);
        Task<User> DeleteUserAsync(Guid userId);
    }
}
