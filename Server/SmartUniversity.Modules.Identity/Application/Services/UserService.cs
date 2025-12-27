using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Security;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Modules.Identity.Domain.Repository;

namespace SmartUniversity.Modules.Identity.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public UserServices(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<UserResponse> RegisterAsync(CreateUserRequest request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
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
                await _userRepository.AddAsync(user);
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

        public async Task<(UserResponse user, string refreshToken, string accessToken)> LoginAsync(
            LoginRequest request
        )
        {
            // first get user;
            bool userExist = await _userRepository.ExistsByEmailAsync(request.Email);
            if (!userExist)
            {
                throw new UserNotFoundException("User with this email doesn't exists!");
            }
            User user;
            try
            {
                user = await _userRepository.GetUserByEmailAsync(request.Email);
            }
            catch (Exception ex)
            {
                throw new AppException("Error while logging in user", ex);
            }

            if (user == null)
            {
                throw new UserNotFoundException("User with this email doesn't exists!");
            }

            string hashedPassword = user.PasswordHash;
            bool match;
            try
            {
                match = _passwordHasher.Verify(request.Password, user.PasswordHash);
            }
            catch (Exception)
            {
                throw new AppException("Login Failed");
            }

            if (!match)
            {
                throw new UserNotFoundException("Wrong Credential is provided!");
            }

            Guid userId = user.Id;
            string role = user.Role.ToString();
            string email = user.Email;
            string accessToken = _jwtService.GenerateToken(userId, email, role, TokenType.Access);
            string refreshToken = _jwtService.GenerateToken(userId, email, role, TokenType.Refresh);

            UserResponse u = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Role = user.Role,
            };

            return (u, refreshToken, accessToken);
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
            bool userExist = await _userRepository.ExistsByIdAsync(userId);
            if (!userExist)
            {
                throw new UserNotFoundException();
            }
            UserResponse res;
            try
            {
                User? user = await _userRepository.DeactivateUserAccount(userId);
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
