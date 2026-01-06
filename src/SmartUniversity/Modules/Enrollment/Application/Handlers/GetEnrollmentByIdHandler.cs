using MediatR;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Enrollment.Application.Handlers
{
    public class GetEnrollmentByIdHandler 
        : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDetailsResponse?>
    {
        private readonly IEnrollmentRepository _repository;

        public GetEnrollmentByIdHandler(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnrollmentDetailsResponse?> Handle(
            GetEnrollmentByIdQuery request,
            CancellationToken ct)
        {
            var enrollment = await _repository.GetAsync(request.EnrollmentId, ct);

            if (enrollment == null)
                return null;

            return new EnrollmentDetailsResponse
            {
                EnrollmentId = enrollment.Id,
                EnrollmentDate = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString(),
                ProgressPercentage = (double)enrollment.ProgressPercentage,

                Course = new EnrollmentCourseInfo
                {
                    CourseId = enrollment.CourseId,
                    Title = "Course info not available",
                    Code = "N/A",
                    InstructorFullName = "N/A",
                    StartDate = DateTime.MinValue,
                    EndDate = DateTime.MinValue,
                    IsPublished = false
                },

                LastAccessed = null
            };
        }
    }
}
