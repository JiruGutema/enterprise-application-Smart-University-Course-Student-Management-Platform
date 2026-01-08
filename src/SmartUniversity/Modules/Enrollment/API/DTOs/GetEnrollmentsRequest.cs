namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class GetMyEnrollmentsRequest
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

}
