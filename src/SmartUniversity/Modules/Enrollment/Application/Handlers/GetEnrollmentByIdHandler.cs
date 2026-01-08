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

    bool isAdmin = request.Roles.Contains("Admin");
    bool isStudent = request.Roles.Contains("Student");
    bool isInstructor = request.Roles.Contains("Instructor");

    // Student: must own the enrollment
    if (isStudent && enrollment.StudentId != request.UserId)
        throw new UnauthorizedAccessException();

    // Instructor: must teach the course (stub for now)
    if (isInstructor && !isAdmin)
    {
        // if (!teachesCourse) throw new UnauthorizedAccessException();
    }

    return new EnrollmentDetailsResponse
    {
        EnrollmentId = enrollment.Id,
          StudentId = enrollment.StudentId,
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
