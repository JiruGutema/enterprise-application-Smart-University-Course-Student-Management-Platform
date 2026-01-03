using SmartUniversity.Modules.Notification.Domain.Entities;
namespace SmartUniversity.Modules.Notification.Application.Interfaces
{
    public interface INotificationServices
    {
        Task CreateNotificationAsync(Notifications notification);
    }
}
