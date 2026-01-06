using MediatR;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;

public class AdminSearchEnrollmentsHandler
    : IRequestHandler<AdminSearchEnrollmentsQuery, AdminEnrollmentsResponse>
{
    private readonly IEnrollmentRepository _repository;

    public AdminSearchEnrollmentsHandler(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminEnrollmentsResponse> Handle(
        AdminSearchEnrollmentsQuery request,
        CancellationToken ct)
    {
        var enrollments = await _repository.AdminSearchAsync(
            request.StudentId,
            request.CourseId,
            request.Status,
            request.Page,
            request.PageSize,
            ct);

        var total = await _repository.AdminCountAsync(
            request.StudentId,
            request.CourseId,
            request.Status,
            ct);

        return new AdminEnrollmentsResponse
        {
            Data = enrollments.Select(e => new AdminEnrollmentRow
            {
                EnrollmentId = e.Id,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollmentDate = e.EnrolledAt,
                Status = e.Status.ToString(),
                ProgressPercentage = (double)e.ProgressPercentage
            }).ToList(),
            Total = total
        };
    }
}
