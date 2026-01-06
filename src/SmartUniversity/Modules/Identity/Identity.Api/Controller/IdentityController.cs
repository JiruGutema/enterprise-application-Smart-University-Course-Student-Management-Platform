using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Interfaces;

namespace SmartUniversity.Modules.Identity.Api.Controllers
{
    [Controller]
    [Route("api/identity")]
    public class IdentityControllers : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ICookieService _cookieServices;

        public IdentityControllers(IUserServices userServices, ICookieService cookieServices)
        {
            _userServices = userServices;
            _cookieServices = cookieServices;
        }

        /// <summary>
        /// Register a user with the default role of the Students. this can be
        /// done by the student itself.
        /// </summary>
        [HttpPost("auth/register")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var user = await _userServices.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { data = user });
        }

        /// <summary>
        /// Login the user with the email and password. default password will be
        /// given by the admin if the admin created the user account
        /// </summary>
        [HttpPost("auth/login")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (user, refreshToken, accessToken) = await _userServices.LoginAsync(request);

            _cookieServices.SetLoginCookies(Response, accessToken, refreshToken);
            return Ok(new { data = user });
        }

        /// <summary>
        /// Refreshes the access token and returns the refreshed access token
        /// via cookies.
        /// </summary>
        [HttpGet("auth/refresh")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken is null)
                return Unauthorized();

            var result = await _userServices.RefreshAccessTokenAsync(refreshToken);
            _cookieServices.SetLoginCookies(
                Response,
                result.newAccessToken,
                result.newRefreshToken
            );

            return Ok(new { message = "success" });
        }

        /// <summary>
        /// Get a Logged in user information if the user is logged in.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> Me()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            UserResponse user = await _userServices.GetUserByIdAsync(Guid.Parse(userId!));
            return Ok(new { data = user });
        }

        /// <summary>
        /// Admin registers  a user with the specified role. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("users")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserRequest request)
        {
            UserResponse users = await _userServices.AdminCreateUser(request);
            return Ok(new { data = users });
        }

        /// <summary>
        /// Admin deactivates  a user account. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> DeactivateUserAccount(
            [FromRoute] DeactivateUserAccountRequest request
        )
        {
            UserResponse user = await _userServices.DeactivateUserAccountAsync(request);
            return Ok(new { data = user });
        }

        /// <summary>
        /// Admin reactivates  a user account. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/activate")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> ActivateUserAccount(
            [FromRoute] ActivateUserAccountRequest request
        )
        {
            UserResponse user = await _userServices.ActivateUserAccountAsync(request);
            return Ok(new { data = user });
        }

        /// <summary>
        /// Admin searches for a user account with a fullname or email. this can be
        /// done  only by the admin.
        /// </summary>
        [Authorize]
        [HttpGet("users")]
        [ProducesResponseType(typeof(SearchUserResponse), 200)]
        public async Task<IActionResult> SearchUserAsync([FromQuery] SearchUserRequest request)
        {
            SearchUserResponse data = await _userServices.SearchUsersAsync(request);

            return Ok(new { data = data });
        }

        /// <summary>
        /// User updates his/her profile. needs to be logged in.
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfile request)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserResponse user = await _userServices.UpdateUserAsync(request, userId);
            return Ok(new { data = user });
        }

        /// <summary>
        /// admin gets a user information.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("users:id")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> UserProfile([FromQuery] string id)
        {
            UserResponse user = await _userServices.GetUserByIdAsync(Guid.Parse(id!));
            return Ok(new { data = user });
        }

        /// <summary>
        /// admin gets a user information.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id}/role")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> ChangeUserRole(
            [FromRoute] string id,
            [FromBody] UpdateRoleRequest request
        )
        {
            UserResponse user = await _userServices.UpdateUserRoleAsync(request, id);
            return Ok(new { data = user });
        }

        /// <summary>
        /// admin gets a user information.
        /// </summary>
        [Authorize]
        [HttpPatch("user/{email}/password-reset")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> ChangePasswordRequestAsync([FromRoute] string email)
        {
            bool sent = await _userServices.ResetPasswordRequestAsync(email);
            if (sent)
            {
                return Ok(new { data = "We have sent you reset link to the email" });
            }

            return StatusCode(500, new { error = "Failed to send email." });
        }
    }
}
