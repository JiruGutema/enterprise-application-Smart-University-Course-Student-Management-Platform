using MediatR;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
namespace SmartUniversity.Modules.Enrollment.Application.Queries;


public class AdminSearchEnrollmentsQuery : IRequest<AdminEnrollmentsResponse>
{
    public Guid? StudentId { get; set; }
    public Guid? CourseId { get; set; }
    public string? Status { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
