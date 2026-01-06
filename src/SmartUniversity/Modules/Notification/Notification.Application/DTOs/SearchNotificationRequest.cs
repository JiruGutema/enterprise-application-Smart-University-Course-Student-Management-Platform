
namespace SmartUniversity.Modules.Notification.Application.DTO
{
    public class SearchNotificationRequest
    {
        public string Query { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
