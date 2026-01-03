using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Modules.Notification.Domain.Repository;
using SmartUniversity.Modules.Notification.Infrastructure.Exceptions;

namespace SmartUniversity.Modules.Notification.Infrastructure.Persistence
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _notificationDbContext;

        public NotificationRepository(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }

        public async Task CreateNotificationAsync(Notifications notification)
        {
            try
            {
                _notificationDbContext.Notifications.Add(notification);
                await _notificationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateNotificationException("Error saving notification.", ex);
            }
        }
    }
}
