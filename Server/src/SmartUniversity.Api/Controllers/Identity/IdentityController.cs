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
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var user = await _userServices.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
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
    }
}
