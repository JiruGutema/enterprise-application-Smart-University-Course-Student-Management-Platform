using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Modules.Notification.Domain.Events;

namespace SmartUniversity.Modules.Notification.Application.Events
{
    public class UserLoggedInEventHandler
    {
        private readonly ILogger<UserLoggedInEventHandler> _logger;
        private readonly IEmailServices _emailServices;
        private readonly INotificationServices _notificationServices;

        public UserLoggedInEventHandler(
            IEmailServices emailServices,
            INotificationServices notificationServices,
            ILogger<UserLoggedInEventHandler> logger
        )
        {
            _emailServices = emailServices;
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(UserLoggedInEvent evt)
        {
            _logger.LogInformation("UserLoggedInEvent received for {Email}", evt.Email);

            await _emailServices.SendLoginDetectedEmailAsync(evt);
            string title = "Registration Successful";

            string message =
                $"Login detected from {evt.Location} at {evt.LoginTime:yyyy-MM-dd HH:mm:ss}. If this wasn't you, please secure your account.";
            Notifications notification = new Notifications(
                evt.UserId,
                title,
                message,
                Domain.Enums.NotificationType.Info
            );
            await _notificationServices.CreateNotificationAsync(notification);
        }
    }
}
