using Microsoft.Extensions.Logging;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Modules.Notification.Domain.Repository;
using SmartUniversity.Modules.Notification.Infrastructure.Exceptions;

namespace SmartUniversity.Modules.Notification.Application.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationServices> _logger;

        public NotificationServices(
            INotificationRepository notificationRepository,
            ILogger<NotificationServices> logger
        )
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(Notifications notification)
        {
            try
            {
                await _notificationRepository.CreateNotificationAsync(notification);
            }
            catch (CreateNotificationException ex)
            {
                _logger.LogError(ex, "Error while creating notification");

                throw new Exception("Error creating notification.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating notification");

                throw new Exception(
                    "An unexpected error occurred while creating notification.",
                    ex
                );
            }
        }
    }
}
