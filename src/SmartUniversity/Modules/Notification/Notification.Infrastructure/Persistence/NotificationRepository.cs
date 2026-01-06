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

        public async Task<PagedResult<Notifications>> SearchNotificationsAsync(
            string query,
            int page,
            int pageSize,
            Guid userId
        )
        {
            var baseQuery = _notificationDbContext.Notifications.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                baseQuery = baseQuery.Where(u =>
                    u.Title.Contains(query) || u.Message.Contains(query) || u.UserId == userId
                );
            }

            var totalCount = await baseQuery.CountAsync();

            var users = await baseQuery
                .OrderBy(u => u.CreatedAt) // optional: consistent order
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Notifications> { Items = users, TotalCount = totalCount };
        }

        public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            Notifications notification = await _notificationDbContext.Notifications.FirstAsync(n =>
                n.Id == notificationId
            );
            if (notification == null)
            {
                throw new NotificationNotFoundException("Notification not found.");
            }

            if (userId != notification.UserId)
            {
                throw new NotificationNotFoundException("Notification not found for this user.");
            }

            _notificationDbContext.Notifications.Remove(notification);
            await _notificationDbContext.SaveChangesAsync();
        }

        public async Task MarkAllAsReadNotificationAsync(Guid userId)
        {
            var notifications = _notificationDbContext
                .Notifications.Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
            }
            await _notificationDbContext.SaveChangesAsync();
        }
    }
}
