using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Notification.Domain.Entities;
using SmartUniversity.Modules.Notification.Domain.Repository;
using SmartUniversity.Modules.Notification.Infrastructure.Exceptions;
using SmartUniversity.Shared.Pagination;

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

        public async Task<PagedResult<Notifications>> GetNotificationByUserIdAsync(
            Guid userId,
            int page,
            int pageSize
        )
        {
            var baseQuery = _notificationDbContext.Notifications.AsQueryable();

            baseQuery = baseQuery.Where(u => u.UserId == userId);

            var totalCount = await baseQuery.CountAsync();

            var notifications = await baseQuery
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Notifications>
            {
                Items = notifications,
                TotalCount = totalCount,
            };
        }

        public async Task<Notifications> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            Notifications notification = _notificationDbContext.Notifications.First(n =>
                n.Id == notificationId
            );

            if (userId != notification.UserId)
            {
                throw new NotificationNotFoundException("Notification not found for this user.");
            }

            notification.MarkAsRead();
            _notificationDbContext.Notifications.Update(notification);
            await _notificationDbContext.SaveChangesAsync();
            return notification;
        }

        public async Task<Notifications> GetNotificationByIdAsync(Guid notificationId, Guid userId)
        {
            Notifications notification = _notificationDbContext.Notifications.First(n =>
                n.Id == notificationId
            );

            if (userId != notification.UserId)
            {
                throw new NotificationNotFoundException("Notification not found for this user.");
            }

            notification.MarkAsRead();
            _notificationDbContext.Notifications.Update(notification);
            await _notificationDbContext.SaveChangesAsync();
            return notification;
        }
    }
}
