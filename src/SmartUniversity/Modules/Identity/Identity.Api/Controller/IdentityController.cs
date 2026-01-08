using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Identity.Application.Commands;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Queries;

namespace SmartUniversity.Modules.Identity.Api.Controllers
{
    [Controller]
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserServices _userServices;
        private readonly ICookieService _cookieServices;

        public IdentityController(IMediator mediator, IUserServices userServices, ICookieService cookieServices)
        {
            _mediator = mediator;
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
            var command = new RegisterUserCommand(request.Email, request.FullName, request.Password);
            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), new { data = user });
        }

        /// <summary>
        /// Login the user with the email and password. default password will be
        /// given by the admin if the admin created the user account
        /// </summary>
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var (user, refreshToken, accessToken) = await _mediator.Send(command);
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
        public async Task<IActionResult> Me()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetUserByIdQuery(Guid.Parse(userId!));
            var user = await _mediator.Send(query);
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
        public async Task<IActionResult> DeactivateUser([FromRoute] string id)
        {
            var command = new DeactivateUserCommand(id);
            var user = await _mediator.Send(command);
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
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUserRequest request)
        {
            var query = new SearchUsersQuery(request.Query, request.Page, request.PageSize);
            var data = await _mediator.Send(query);
            return Ok(new { data });
        }

        /// <summary>
        /// User updates his/her profile. needs to be logged in.
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfile request)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new UpdateUserCommand(userId!, request.Email, request.FullName, request.Password);
            var user = await _mediator.Send(command);
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
        /// reset password endpoint. this can also be used for forgot password
        /// </summary>
        [HttpGet("password-reset-request")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ChangePasswordRequestAsync([FromQuery] string email)
        {
            bool sent = await _userServices.ResetPasswordRequestAsync(email);
            if (sent)
            {
                return Ok(new { message = "We have sent you reset link to the email" });
            }

            return StatusCode(500, new { error = "Failed to send email." });
        }

        /// <summary>
        /// verify change password. new password and resettoken should be sent
        /// via body
        /// </summary>
        [HttpPost("password-reset/confirm")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ResetPasswordRequest request
        )
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (user, refreshToken, accessToken) = await _userServices.ResetPasswordAsync(
                request,
                userId
            );

            _cookieServices.SetLoginCookies(Response, accessToken, refreshToken);
            return Ok(new { data = user });
        }

        /// <summary>
        /// Admin Deletes users by id
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("user/delete/:id")]
        [ProducesResponseType(typeof(UserResponseWrapper), 200)]
        public async Task<IActionResult> ChangePasswordAsync([FromQuery] string id)
        {
            UserResponse user = await _userServices.DeleteUserAsync(id);
            return Ok(new { data = user });
        }
    }
}
