using MediatR;
using SmartUniversity.Modules.Courses.Application.DTOs;
using SmartUniversity.Modules.Courses.Application.Queries;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class GetCoursesByCodesHandler : IRequestHandler<GetCoursesByCodesQuery, IEnumerable<CourseResponse>>
{
    private readonly ICourseRepository _repository;

    public GetCoursesByCodesHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CourseResponse>> Handle(GetCoursesByCodesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repository.GetByCodesAsync(request.Codes);
        return courses.Select(CourseResponse.FromDomain);
    }
}
