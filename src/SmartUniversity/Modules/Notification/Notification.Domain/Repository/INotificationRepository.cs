using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Domain.Repository
{
    public interface INotificationRepository
    {
        Task CreateNotificationAsync(Notifications notification);
    }
}
