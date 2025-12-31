using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Domain.Enums;

namespace SmartUniversity.Modules.Identity.Api.Conrollers
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
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var user = await _userServices.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), user);
        }

        /// <summary>
        /// Login the user with the email and password. default password will be
        /// given by the admin if the admin created the user account
        /// </summary>
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (user, refreshToken, accessToken) = await _userServices.LoginAsync(request);

            _cookieServices.SetLoginCookies(Response, accessToken, refreshToken);
            return Ok(new { User = user });
        }

        /// <summary>
        /// Refreshes the access token and returns the refreshed access token
        /// via cookies.
        /// </summary>
        [HttpPost("auth/refresh")]
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
        public async Task<IActionResult> Me()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            UserResponse user = await _userServices.GetUserByIdAsync(Guid.Parse(userId!));
            return Ok(new { user = user });
        }

        /// <summary>
        /// Admin registers  a user with the specified role. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "2")]
        [HttpPost("users")]
        public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserRequest request)
        {
            UserResponse users = await _userServices.AdminCreateUser(request);
            return Ok(new { users = users });
        }

        /// <summary>
        /// Admin deactivates  a user account. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "2")]
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateUserAccount(
            [FromQuery] DeactivateUserAccountRequest request
        )
        {
            UserResponse user = await _userServices.DeactivateUserAccountAsync(request);
            return Ok(new { user = user });
        }

        /// <summary>
        /// Admin reactivates  a user account. this can be
        /// done  only by the admin itself.
        /// </summary>
        [Authorize(Roles = "2")]
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateUserAccount(
            [FromQuery] ActivateUserAccountRequest request
        )
        {
            UserResponse user = await _userServices.ActivateUserAccountAsync(request);
            return Ok(new { user = user });
        }
    }
}
