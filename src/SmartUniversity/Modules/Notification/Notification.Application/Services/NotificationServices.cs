using SmartUniversity.Modules.Notification.Application.DTO;
using SmartUniversity.Modules.Notification.Application.Exceptions;
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
            if (userId.IsWhiteSpace() || userId == null)
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

        public async Task<NotificationResponse> MarkAsReadAsync(string nId, string uId)
        {
            if (nId == null)
            {
                throw new GetNotificationException("UserId cannot be null.");
            }

            if (uId == null)
            {
                throw new GetNotificationException("UserId cannot be null.");
            }

            Guid Id = Guid.Parse(nId);
            Guid userId = Guid.Parse(uId);
            try
            {
                Notifications notification = await _notificationRepository.MarkAsReadAsync(
                    Id,
                    userId
                );
                NotificationResponse res = new NotificationResponse
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt,
                    UserId = notification.UserId,
                    Type = notification.Type,
                };
                return res;
            }
            catch (Exception ex)
            {
                throw new GetNotificationException("Error marking as read");
            }
        }

        public async Task<NotificationResponse> GetNotificationByIdAsync(string nId, string uId)
        {
            if (nId == null)
            {
                throw new GetNotificationException("Notification id is required.");
            }

            if (uId == null)
            {
                throw new UnauthorizedAccessException("Unauthorized Access.");
            }

            Guid notificationId = Guid.Parse(nId);

            Guid userId = Guid.Parse(uId);
            try
            {
                Notifications notification = await _notificationRepository.GetNotificationByIdAsync(
                    notificationId,
                    userId
                );
                NotificationResponse res = new NotificationResponse
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt,
                    UserId = notification.UserId,
                    Type = notification.Type,
                };
                return res;
            }
            catch (Exception ex)
            {
                throw new GetNotificationException("Error marking as read");
            }
        }

        public async Task<SearchNotificationResponse> SearchNotificationsAsync(
            SearchNotificationRequest request,
            string userId
        )
        {
            if (userId is null)
            {
                throw new AppException("UserId cannot be null.");
            }
            Guid uId = Guid.Parse(userId);

            var result = await _notificationRepository.SearchNotificationsAsync(
                request.Query,
                request.Page,
                request.PageSize,
                uId
            );

            var notifcations = result
                .Items.Select(u => new NotificationResponse
                {
                    Id = u.Id,
                    Title = u.Title,
                    Message = u.Message,
                    UserId = u.UserId,
                    IsRead = u.IsRead,
                    CreatedAt = u.CreatedAt,
                })
                .ToList();

            return new SearchNotificationResponse
            {
                Data = notifcations,
                Total = result.TotalCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };
        }
    }
}
