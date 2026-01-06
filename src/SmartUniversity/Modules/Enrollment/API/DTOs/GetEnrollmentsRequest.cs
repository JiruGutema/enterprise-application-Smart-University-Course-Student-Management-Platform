// WILL REMOVE STUDENTID WHEN JWT IS COMPLETE
namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class GetMyEnrollmentsRequest
    {
        public Guid StudentId { get; set; }  // i can later read from JWT
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
