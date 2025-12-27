using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Modules.Identity.Infrastructure.Exceptions;

namespace SmartUniversity.Modules.Identity.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // by id
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching user by ID", ex);
            }
        }

        // check with id
        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        // by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding user", ex);
            }
        }

        public async Task<User> DeactivateUserAccount(Guid id)
        {
            User user = _context.Users.First(u => u.Id == id);
            user.Deactivate();
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DeactiveUserException("Error deactivating user");
            }
            return user;
        }
    }
}
