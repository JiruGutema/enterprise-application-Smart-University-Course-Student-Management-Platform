using SmartUniversity.Modules.Identity.Domain.Entities;
namespace SmartUniversity.Modules.Identity.Domain.Repository

{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByIdAsync(Guid Id);
        Task<User> DeactivateUserAccount(Guid Id);
        Task AddAsync(User user);
    }
}
