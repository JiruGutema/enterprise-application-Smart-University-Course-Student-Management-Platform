using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Shared.Pagination;

namespace SmartUniversity.Modules.Notification.Domain.Repository
{
    public interface INotificationRepository
    {
        Task CreateNotificationAsync(Notifications notification);

        Task<PagedResult<Notifications>> GetNotificationByUserIdAsync(
            Guid userId,
            int page,
            int pageSize
        );

        Task<Notifications> MarkAsReadAsync(Guid id, Guid userId);

        Task<Notifications> GetNotificationByIdAsync(Guid id, Guid userId); 
    }
}
