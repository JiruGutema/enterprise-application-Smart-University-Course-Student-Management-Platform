using SmartUniversity.Modules.Notification.Application.DTO;
using SmartUniversity.Modules.Notification.Domain.Entities;

namespace SmartUniversity.Modules.Notification.Application.Interfaces
{
    public interface INotificationServices
    {
        Task CreateNotificationAsync(Notifications notification);
        Task<GetNotificationResponse> GetNotificationsByUserIdAsync(
            string? userId,
            GetNotificationRequest request
        );
        Task<NotificationResponse> MarkAsReadAsync(string notificationId, string? userId);

        Task<NotificationResponse> GetNotificationByIdAsync(string notificationId, string? userId);

        Task<SearchNotificationResponse> SearchNotificationsAsync(
            SearchNotificationRequest request,
            string userId
        );
        Task DeleteNotificationAsync(string notificationId, string? userId);

    }
}
