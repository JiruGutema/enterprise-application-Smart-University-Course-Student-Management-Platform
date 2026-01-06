using MediatR;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Enrollment.Application.Handlers
{
    public class GetMyEnrollmentsHandler : IRequestHandler<GetMyEnrollmentsQuery, MyEnrollmentsResponse>
    {
        private readonly IEnrollmentRepository _repository;

        public GetMyEnrollmentsHandler(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<MyEnrollmentsResponse> Handle(GetMyEnrollmentsQuery request, CancellationToken ct)
        {
            var enrollments = await _repository.GetByStudentAsync(
                request.StudentId,
                request.Status,
                request.Page,
                request.PageSize,
                ct
            );

            var total = await _repository.CountByStudentAsync(request.StudentId, request.Status, ct);

          var response = new MyEnrollmentsResponse
{
    Data = enrollments.Select(e => new EnrollmentDetailsResponse
    {
        EnrollmentId = e.Id,
        EnrollmentDate = e.EnrolledAt,
        Status = e.Status.ToString(),
        ProgressPercentage = (double)e.ProgressPercentage,
        Course = new EnrollmentCourseInfo
        {
            CourseId = e.CourseId,
            Title = "Course info not available",
            Code = "N/A",
            InstructorFullName = "N/A",
            StartDate = DateTime.MinValue,
            EndDate = DateTime.MinValue,
            IsPublished = false
        }
    }).ToList(),
    Total = total
};


            return response;
        }
    }
}
