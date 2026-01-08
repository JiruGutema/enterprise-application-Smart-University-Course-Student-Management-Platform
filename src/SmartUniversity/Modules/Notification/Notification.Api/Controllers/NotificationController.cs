using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Notification.Application.DTO;
using SmartUniversity.Modules.Notification.Application.Interfaces;

namespace SmartUniversity.Modules.Notification.Api.Controllers
{
    [Controller]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;

        public NotificationController(INotificationServices notificationServices)
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
        /// Mark as read notification by id.
        /// failure
        /// </summary>
        [Authorize]
        [HttpPatch("{id}/mark-as-read")]
        [ProducesResponseType(typeof(NotificationResponse), 200)]
        public async Task<IActionResult> MarkAsReadAsync([FromRoute] string id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _notificationServices.MarkAsReadAsync(id, userId);
            return Ok(new { notification = notification });
        }

        /// <summary>
        /// get notification by id.
        /// </summary>
        [Authorize]
        [HttpGet(":id")]
        [ProducesResponseType(typeof(NotificationResponse), 200)]
        public async Task<IActionResult> GetNotificationByIdAsync([FromQuery] string id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _notificationServices.GetNotificationByIdAsync(id, userId);
            return Ok(new { notification = notification });
        }

        /// <summary>
        /// User searches for a notification with a title or message content.
        /// </summary>
        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(typeof(SearchNotificationResponse), 200)]
        public async Task<IActionResult> SearchUserAsync(
            [FromQuery] SearchNotificationRequest request
        )
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            SearchNotificationResponse data = await _notificationServices.SearchNotificationsAsync(
                request,
                userId!
            );

            return Ok(new { data = data });
        }

        /// <summary>
        /// User searches for a notification with a title or message content.
        /// </summary>
        [Authorize]
        [HttpDelete(":id")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> DeleteNotificationAsync([FromQuery] string id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationServices.DeleteNotificationAsync(id, userId!);

            return Ok();
        }

        /// <summary>
        /// User marks all unread notification as read.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("mark-all-as-read")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> MarkAllAsReadNotificationAsync()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationServices.MarkAllAsReadNotificationAsync(userId!);

            return Ok();
        }
    }
}
