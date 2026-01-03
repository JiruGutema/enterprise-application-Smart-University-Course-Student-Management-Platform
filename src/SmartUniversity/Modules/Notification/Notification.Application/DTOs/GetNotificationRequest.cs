namespace SmartUniversity.Modules.Notification.Application.DTO
{
    public class GetNotificationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
