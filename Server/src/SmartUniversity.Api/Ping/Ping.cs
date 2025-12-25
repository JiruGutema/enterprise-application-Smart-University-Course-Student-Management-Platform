using Microsoft.AspNetCore.Mvc;

namespace SmartUniversity.Api.Controllers.Ping
{
    [Controller]
    [Route("api/ping")]
    public class UserControllers : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new { message = "Pong" });
        }
    }
}
