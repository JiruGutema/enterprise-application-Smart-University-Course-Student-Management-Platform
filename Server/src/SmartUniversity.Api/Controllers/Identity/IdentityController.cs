using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Interfaces;

namespace SmartUniversity.Api.Controllers.Users
{
    [Controller]
    [Route("api/auth")]
    public class UserControllers : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ICookieService _cookieServices;

        public UserControllers(IUserServices userServices, ICookieService cookieServices)
        {
            _userServices = userServices;
            _cookieServices = cookieServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            var user = await _userServices.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (user, refreshToken, accessToken) = await _userServices.LoginAsync(request);

            _cookieServices.SetLoginCookies(Response, accessToken, refreshToken);
            return Ok(new { User = user });
        }

        [HttpPost("refresh")]
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
    }
}
