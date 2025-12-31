using Microsoft.AspNetCore.Mvc;

namespace SmartUniversity.Api.Controllers.Ping
{
    [Controller]
    [Route("api/ping")]
    public class PingControllers : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new { message = "Pong" });
        }
    }
}
