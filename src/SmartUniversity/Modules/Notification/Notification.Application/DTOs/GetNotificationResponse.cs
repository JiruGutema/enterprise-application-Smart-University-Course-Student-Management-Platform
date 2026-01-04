namespace SmartUniversity.Modules.Notification.Application.DTO
{
    public class GetNotificationResponse
    {
        public List<NotificationResponse> Notification { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

}
