using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userrePository;
        private readonly IPasswordHasher _passwordHasher;

        public UserServices(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userrePository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponse> RegisterAsync(CreateUserRequest request)
        {
            if (await _userrePository.ExistsByEmailAsync(request.Email))
            {
                throw new UserAlreadyExistsException("Email Already Exist");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(
                Guid.NewGuid(),
                request.Email,
                request.FullName,
                Role.Student,
                passwordHash
            );
            try
            {
                await _userrePository.AddAsync(user);
            }
            catch (Exception ex)
            {
                throw new UserAlreadyExistsException("Error creating user", ex);
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
            };
        }

        public async Task<UserResponse> DeactivateUserAccountAsync(
            DeactivateUserAccountRequest request
        )
        {
            Guid userId = Guid.Parse(request.Id);
            if (userId == Guid.Empty)
            {
                throw new InvalidUserException("User Id is required to deactivate user");
            }
            bool userExist = await _userrePository.ExistsByIdAsync(userId);
            if (!userExist)
            {
                throw new UserNotFoundException();
            }
            UserResponse res;
            try
            {
                User? user = await _userrePository.DeactivateUserAccount(userId);
                res = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive,
                };
            }
            catch (Exception e)
            {
                throw new DeactiveUserException("Error deactivating user account", e);
            }

            return res;
        }
    }
}
