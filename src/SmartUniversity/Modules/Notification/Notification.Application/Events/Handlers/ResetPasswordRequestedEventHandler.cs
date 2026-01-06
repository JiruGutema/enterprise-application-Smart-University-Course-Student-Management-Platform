using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.Events
{
    public class ResetPasswordRequestedEventHandler
    {
        private readonly ILogger<ResetPasswordRequestedEventHandler> _logger;
        private readonly IEmailServices _emailServices;
        private readonly INotificationServices _notificationServices;

        public ResetPasswordRequestedEventHandler(
            IEmailServices emailServices,
            INotificationServices notificationServices,
            ILogger<ResetPasswordRequestedEventHandler> logger
        )
        {
            _emailServices = emailServices;
            _notificationServices = notificationServices;
            _logger = logger;
        }

        public async Task HandleAsync(ResetPasswordRequestedEvent evt)
        {
            _logger.LogInformation("ResetPasswordRequestedEvent received for {Email}", evt.Email);

            await _emailServices.SendResetPasswordEmailAsync(evt);
            string title = "Password reset request";
            string message =
                "we have sent you a reset link via email. please, use the link to change your password. if you haven't asked, just ignore the message";
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
