namespace SmartUniversity.Modules.Enrollment.Api.DTOs;

public class AdminEnrollmentRow
{
    public Guid EnrollmentId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ProgressPercentage { get; set; }
}

public class AdminEnrollmentsResponse
{
    public List<AdminEnrollmentRow> Data { get; set; } = new();
    public int Total { get; set; }
}
