namespace SmartUniversity.Modules.Notification.Application.DTO
{
    public class SearchNotificationResponse
    {
        public List<NotificationResponse> Data { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchUserResponseWrapper
    {
        public SearchNotificationResponse Data { get; set; }
    }
}
