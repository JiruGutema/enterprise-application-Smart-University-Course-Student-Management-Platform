using MediatR;
using SmartUniversity.Modules.Enrollment.Api.DTOs;

namespace SmartUniversity.Modules.Enrollment.Application.Queries
{
    public class GetStudentsByCourseQuery : IRequest<CourseEnrollmentStudentsResponse>
    {
        public Guid CourseId { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
