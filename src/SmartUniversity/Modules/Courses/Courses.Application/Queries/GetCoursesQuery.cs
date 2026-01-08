using MediatR;
using SmartUniversity.Modules.Courses.Application.Common;
using SmartUniversity.Modules.Courses.Application.DTOs;

namespace SmartUniversity.Modules.Courses.Application.Queries
{
    
    public class GetCoursesQuery : IRequest<PagedResult<CourseResponse>>
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool PublishedOnly { get; init; } = false;
        public Guid? InstructorId { get; init; }
        public string? Search { get; init; }
        public bool IncludeUnpublished { get; init; } = false;
    }
}
