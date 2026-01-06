namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class ChangeEnrollmentStatusRequest
    {
        public string Status { get; set; } = string.Empty; // "Completed" | "Withdrawn"
    }
}
