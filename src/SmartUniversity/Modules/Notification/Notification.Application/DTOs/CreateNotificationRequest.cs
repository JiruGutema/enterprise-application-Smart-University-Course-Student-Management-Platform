using SmartUniversity.Modules.Notification.Domain.Enums;

namespace SmartUniversity.Modules.Notification.Application.DTO
{
    public class CreateNotificationRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead {get; set;}
    }
}
