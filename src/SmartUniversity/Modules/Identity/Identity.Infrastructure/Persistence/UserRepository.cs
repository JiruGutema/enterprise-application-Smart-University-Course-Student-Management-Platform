using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Modules.Identity.Infrastructure.Exceptions;
using SmartUniversity.Shared.Pagination;

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
        public async Task<User?> GetUserByEmailAsync(string email)
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
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> ActivateUserAccount(Guid id)
        {
            User user = _context.Users.First(u => u.Id == id);
            user.Activate();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PagedResult<User>> SearchUsersAsync(string query, int page, int pageSize)
        {
            var baseQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                baseQuery = baseQuery.Where(u =>
                    u.FullName.Contains(query) || u.Email.Contains(query)
                );
            }

            var totalCount = await baseQuery.CountAsync();

            var users = await baseQuery
                .OrderBy(u => u.FullName) // optional: consistent order
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User> { Items = users, TotalCount = totalCount };
        }

        public async Task<User> UpdateUserAsync(
            string? email,
            string? fullName,
            string? passwordHash,
            Guid id
        )
        {
            if (email != null)
            {
                await _context
                    .Users.Where(u => u.Id == id)
                    .ExecuteUpdateAsync(u => u.SetProperty(x => x.Email, email));
            }

            if (fullName != null)
            {
                await _context
                    .Users.Where(u => u.Id == id)
                    .ExecuteUpdateAsync(u => u.SetProperty(x => x.FullName, fullName));
            }
            if (passwordHash != null)
            {
                await _context
                    .Users.Where(user => user.Id == id)
                    .ExecuteUpdateAsync(u => u.SetProperty(x => x.PasswordHash, passwordHash));
            }

            await _context.SaveChangesAsync();
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User> UpdateUserRoleAsync(Role role, Guid id)
        {
            if (!Enum.IsDefined(typeof(Role), role))
            {
                throw new InvalidRoleException();
            }
            await _context
                .Users.Where(user => user.Id == id)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.Role, role));
            await _context.SaveChangesAsync();
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
    }
}
