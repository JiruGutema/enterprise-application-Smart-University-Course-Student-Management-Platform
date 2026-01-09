using MediatR;
using SmartUniversity.Modules.Content.API.DTOs;

namespace SmartUniversity.Modules.Content.Application.Queries;

public class GetMaterialsByCourseQuery : IRequest<PagedResult<MaterialDto>>
{
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public string? FileType { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}