using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Identity.Application.DTO;
using SmartUniversity.Modules.Identity.Application.Interfaces;

namespace SmartUniversity.Api.Controllers.Users
{
    [Controller]
    [Route("api/users")]
    public class UserControllers : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserControllers(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var user = await _userServices.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), user);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new { message = "Pong" });
        }
    }
}
