namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class EnrollmentCourseInfo
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string InstructorFullName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublished { get; set; }
    }

    public class EnrollmentDetailsResponse
    {
        public Guid EnrollmentId { get; set; }
        public EnrollmentCourseInfo Course { get; set; } = new EnrollmentCourseInfo();
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public DateTime? LastAccessed { get; set; }  // optional
    }

    public class MyEnrollmentsResponse
    {
        public List<EnrollmentDetailsResponse> Data { get; set; } = new List<EnrollmentDetailsResponse>();
        public int Total { get; set; }
    }
}
