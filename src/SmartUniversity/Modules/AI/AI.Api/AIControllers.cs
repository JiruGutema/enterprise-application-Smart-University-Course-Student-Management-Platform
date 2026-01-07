using Microsoft.AspNetCore.Mvc;

namespace SmartUniversity.Modules.AI.Api.Controllers
{
    [Controller]
    [Route("api/ai")]
    public class AIControllers : ControllerBase
    {

      
        /// <summary>
        /// check if the ai end point is mounted
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Check()
        {
            return Ok("working");
        }
    }
}
