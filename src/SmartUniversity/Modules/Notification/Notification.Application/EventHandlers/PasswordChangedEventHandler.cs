using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.EventHandlers
{
    public class PasswordChangedEventHandler
    {
        private readonly ILogger<PasswordChangedEventHandler> _logger;
        private readonly IEmailServices _emailServices;
        private readonly INotificationServices _notificationServices;

        public PasswordChangedEventHandler(
            IEmailServices emailServices,
            INotificationServices notificationServices,
            ILogger<PasswordChangedEventHandler> logger
        )
        {
            _emailServices = emailServices;
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(PasswordChangedEvent evt)
        {
            _logger.LogInformation("PasswordChangedEvent received for {Email}", evt.Email);

            await _emailServices.SendPasswordChangedEmailAsync(evt);
            string title = "Password Reset Successful";

            string message =
                $"Your password has been changed. If it was not you, please contact the registrar as sooon as possible";
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
