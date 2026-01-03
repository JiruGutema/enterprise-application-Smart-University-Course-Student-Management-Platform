using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Notification.Application.DTO;
using SmartUniversity.Modules.Notification.Application.Interfaces;

namespace SmartUniversity.Modules.Notification.Api.Controllers
{
    [Controller]
    [Route("api/notification")]
    public class NotificationControllers : ControllerBase
    {
        private readonly INotificationServices _notificationServices;

        public NotificationControllers(INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }

        /// <summary>
        /// Get paginated user notification for logged in user.
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(GetNotificationResponse), 200)]
        public async Task<IActionResult> GetNotificationByUserId(
            [FromQuery] GetNotificationRequest request
        )
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            GetNotificationResponse response =
                await _notificationServices.GetNotificationsByUserIdAsync(userId, request);
            return Ok(response);
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
