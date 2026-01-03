using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartUniversity.Modules.Notification.Api.Controllers
{
    [Controller]
    [Route("api/notification")]
    public class NotificationControllers : ControllerBase
    {
        /// <summary>
        /// Checks if notification endpoint is working
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Check()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(
                new
                {
                    Notification = new
                    {
                        notifications = "you don't have unread notification for now",
                        userId = userId,
                    },
                }
            );
        }

        /// <summary>
        /// Checks if notification endpoint is working
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> CreateNotificationAsync()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(
                new
                {
                    Notification = new { notifications = "Created Notification", userId = userId },
                }
            );
        }
    }
}
