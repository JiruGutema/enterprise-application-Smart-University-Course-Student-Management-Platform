namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class CourseEnrollmentStudentResponse
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }

        // These are READ MODEL fields (can be null for now) will add them afeter course is finished
        public string? StudentFullName { get; set; }
        public string? StudentEmail { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
    }

    public class CourseEnrollmentStudentsResponse
    {
        public List<CourseEnrollmentStudentResponse> Data { get; set; } = new();
        public int Total { get; set; }
    }
}
