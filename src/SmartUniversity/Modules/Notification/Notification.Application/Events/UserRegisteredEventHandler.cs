using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Application.Services;
using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Modules.Notification.Domain.Events;

namespace SmartUniversity.Modules.Notification.Application.Events
{
    public class UserRegisteredEventHandler
    {
        private readonly ILogger<UserRegisteredEventHandler> _logger;
        private readonly EmailServices _emailServices;
        private readonly INotificationServices _notificationServices;

        public UserRegisteredEventHandler(
            EmailServices emailServices,
            INotificationServices notificationServices,
            ILogger<UserRegisteredEventHandler> logger
        )
        {
            _emailServices = emailServices;
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(UserRegisteredEvent evt)
        {
            _logger.LogInformation("UserRegisteredEvent received for {Email}", evt.Email);

            await _emailServices.SendWelcomeEmailAsync(evt);
            string title = "Registration Successful";
            string message =
                "Welcome! Your registration has been completed successfully. You can now access all features.";
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
