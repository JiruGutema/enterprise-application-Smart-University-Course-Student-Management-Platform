namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class EnrollRequest
    {
        public Guid StudentId { get; set; }   // TEMP: for testing without auth
        public Guid CourseId { get; set; }
    }
}
