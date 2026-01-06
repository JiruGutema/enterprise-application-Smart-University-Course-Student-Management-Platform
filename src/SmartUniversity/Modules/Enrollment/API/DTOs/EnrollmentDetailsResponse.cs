namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class EnrollmentDetailsResponse
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
