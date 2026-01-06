using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Security;
using SmartUniversity.Modules.Identity.Domain.Entities;
using SmartUniversity.Modules.Identity.Domain.Enums;
using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Modules.Identity.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IEventBus _eventBus;

        public UserServices(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IEventBus eventBus
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _eventBus = eventBus;
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
            // publish user registered event
            var userRegisteredEvent = new UserRegisteredEvent(user.Id, user.Email, user.FullName);
            await _eventBus.PublishAsync(userRegisteredEvent);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
            };
        }

        public async Task<UserResponse> GetUserByEmailAsync(string email)
        {
            bool userExist = await _userRepository.ExistsByEmailAsync(email);
            if (!userExist)
            {
                throw new UserNotFoundException();
            }

            User user;
            try
            {
                user = await _userRepository.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                throw new AppException("Error while fetching user by email", ex);
            }

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Role = user.Role.ToString(),
            };
        }

        public async Task<UserResponse> AdminCreateUser(AdminCreateUserRequest request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new UserAlreadyExistsException("Email Already Exist");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(
                id: Guid.NewGuid(),
                email: request.Email,
                fullName: request.FullName,
                role: request.Role,
                passwordHash: passwordHash
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
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
            };
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                throw new AppException("User id is required");
            }
            bool userExist = await _userRepository.ExistsByIdAsync(userId);
            if (!userExist)
            {
                throw new UserNotFoundException();
            }

            User user;
            try
            {
                user = await _userRepository.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new AppException("Error while fetching user by Id", ex);
            }

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Role = user.Role.ToString(),
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
                Role = user.Role.ToString(),
            };

            var userLoggedInEvent = new UserLoggedInEvent(
                user.Id,
                user.Email,
                user.FullName,
                "Addis Ababa, Ethiopia",
                DateTime.Now
            );
            await _eventBus.PublishAsync(userLoggedInEvent);

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
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                };
            }
            catch (Exception e)
            {
                throw new AppException("Error deactivating user account", e);
            }

            // publish user account deacitvated event
            var userAccountDeactivatedEvent = new UserAccountDeactivatedEvent(
                res.Id,
                res.Email,
                res.FullName
            );
            await _eventBus.PublishAsync(userAccountDeactivatedEvent);

            return res;
        }

        public async Task<UserResponse> ActivateUserAccountAsync(ActivateUserAccountRequest request)
        {
            Guid userId = Guid.Parse(request.Id);
            if (userId == Guid.Empty)
            {
                throw new InvalidUserException("User Id is required to activate user");
            }
            bool userExist = await _userRepository.ExistsByIdAsync(userId);
            if (!userExist)
            {
                throw new UserNotFoundException();
            }
            UserResponse res;
            try
            {
                User? user = await _userRepository.ActivateUserAccount(userId);
                res = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("\n here \n");
                throw new ActiveUserException("Error activating user account", e);
            }

            return res;
        }

        public async Task<(string newAccessToken, string newRefreshToken)> RefreshAccessTokenAsync(
            string refreshToken
        )
        {
            bool validate = _jwtService.ValidateToken(
                refreshToken,
                TokenType.Refresh,
                out var userId
            );
            if (!validate)
            {
                throw new UnauthorizedAccessException();
            }

            var result = _jwtService.RefreshAccessToken(refreshToken);

            return (result.newAccessToken, result.newRefreshToken);
        }

        public async Task<SearchUserResponse> SearchUsersAsync(SearchUserRequest request)
        {
            var result = await _userRepository.SearchUsersAsync(
                request.Query,
                request.Page,
                request.PageSize
            );

            var users = result
                .Items.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                })
                .ToList();

            return new SearchUserResponse
            {
                Data = users,
                Total = result.TotalCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };
        }

        public async Task<UserResponse> UpdateUserAsync(UpdateUserProfile request, string userId)
        {
            Guid id = Guid.Parse(userId);
            string? passwordHash = null;
            if (request.Password != null)
            {
                passwordHash = _passwordHasher.Hash(request.Password);
            }

            bool exist = await _userRepository.ExistsByIdAsync(id);
            if (!exist)
            {
                throw new UserNotFoundException();
            }

            User res = await _userRepository.UpdateUserAsync(
                request.Email,
                request.FullName,
                passwordHash,
                id
            );

            UserResponse user = new UserResponse
            {
                Id = res.Id,
                Email = res.Email,
                FullName = res.FullName,
                IsActive = res.IsActive,
                Role = res.Role.ToString(),
            };

            if (request.Email != null)
            {
                //  publish  email changed event
                var userEmailUpdatedEvent = new UserEmailUpdatedEvent(
                    res.Id,
                    res.Email,
                    res.FullName
                );
                await _eventBus.PublishAsync(userEmailUpdatedEvent);
            }

            if (request.FullName != null)
            {
                // publish  email changed event
                var userFullNameUpdatedEvent = new UserFullNameUpdatedEvent(
                    res.Id,
                    res.Email,
                    res.FullName
                );
                await _eventBus.PublishAsync(userFullNameUpdatedEvent);
            }

            return user;
        }

        public async Task<UserResponse> UpdateUserRoleAsync(
            UpdateRoleRequest request,
            string userId
        )
        {
            Guid id = Guid.Parse(userId);
            if (userId is null)
            {
                throw new AppException("user id is required");
            }

            if (request is null)
            {
                throw new AppException("user role is required");
            }

            Role role = request.Role;
            bool exist = await _userRepository.ExistsByIdAsync(id);
            if (!exist)
            {
                throw new UserNotFoundException();
            }
            User res;
            try
            {
                res = await _userRepository.UpdateUserRoleAsync(role, id);
            }
            catch (AppException ex)
            {
                throw new AppException("Invalid user role", ex);
            }

            UserResponse user = new UserResponse
            {
                Id = res.Id,
                Email = res.Email,
                FullName = res.FullName,
                IsActive = res.IsActive,
                Role = res.Role.ToString(),
            };

            return user;
        }
    }
}
