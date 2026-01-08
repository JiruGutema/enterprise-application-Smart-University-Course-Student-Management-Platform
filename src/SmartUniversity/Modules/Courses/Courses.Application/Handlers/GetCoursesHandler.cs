using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Application.Common;
using SmartUniversity.Modules.Courses.Application.DTOs;
using SmartUniversity.Modules.Courses.Application.Queries;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, PagedResult<CourseResponse>>
{
    private readonly ICourseRepository _repository;

    public GetCoursesHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CourseResponse>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var allCourses = await _repository.GetAllAsync();
        var pagedCourses = allCourses
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(CourseResponse.FromDomain)
            .ToList();

        return new PagedResult<CourseResponse>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = allCourses.Count(),
            Items = pagedCourses
        };
    }
}
