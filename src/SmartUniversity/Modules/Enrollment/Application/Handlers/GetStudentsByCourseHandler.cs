using MediatR;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;

namespace SmartUniversity.Modules.Enrollment.Application.Handlers
{
    public class GetStudentsByCourseHandler
        : IRequestHandler<GetStudentsByCourseQuery, CourseEnrollmentStudentsResponse>
    {
        private readonly IEnrollmentRepository _repository;

        public GetStudentsByCourseHandler(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<CourseEnrollmentStudentsResponse> Handle(
            GetStudentsByCourseQuery request,
            CancellationToken ct)
        {
            var enrollments = await _repository.GetByCourseAsync(
                request.CourseId,
                request.Status,
                request.Page,
                request.PageSize,
                ct);

            var total = await _repository.CountByCourseAsync(
                request.CourseId,
                request.Status,
                ct);

            return new CourseEnrollmentStudentsResponse
            {
                Data = enrollments.Select(e => new CourseEnrollmentStudentResponse
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    EnrollmentDate = e.EnrolledAt,
                    Status = e.Status.ToString(),
                    ProgressPercentage = (double)e.ProgressPercentage,

                    // Will fill it later via projection / integration
                    StudentFullName = null,
                    StudentEmail = null
                }).ToList(),

                Total = total
            };
        }
    }
}
