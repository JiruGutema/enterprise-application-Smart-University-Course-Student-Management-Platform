using Microsoft.Extensions.Logging;
using SmartUniversity.Modules.Notification.Application.DTO;
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

        public async Task<GetNotificationResponse> GetNotificationsByUserIdAsync(
            string userId,
            GetNotificationRequest request
        )
        {
            if (userId == null)
            {
                throw new GetNotificationException("UserId cannot be null.");
            }
            try
            {
                Guid UserId = Guid.Parse(userId);

                var res = await _notificationRepository.GetNotificationByUserIdAsync(
                    UserId,
                    request.Page,
                    request.PageSize
                );
                var notification = res
                    .Items.Select(n => new NotificationResponse
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Message = n.Message,
                        CreatedAt = n.CreatedAt,
                        UserId = n.UserId,
                        IsRead = n.IsRead,
                        Type = n.Type,
                    })
                    .ToList();
                return new GetNotificationResponse
                {
                    Notification = notification,
                    Total = res.TotalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                };
            }
            catch (GetNotificationException ex)
            {
                _logger.LogError(
                    ex,
                    "Error while retrieving notifications for user {UserId}",
                    userId
                );

                throw new Exception("Error retrieving notifications.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while retrieving notifications for user {UserId}",
                    userId
                );

                throw new Exception(
                    "An unexpected error occurred while retrieving notifications.",
                    ex
                );
            }
        }
    }
}
