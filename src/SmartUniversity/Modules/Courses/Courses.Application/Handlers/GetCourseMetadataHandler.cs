using MediatR;
using SmartUniversity.Modules.Courses.Application.DTOs;
using SmartUniversity.Modules.Courses.Application.Queries;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Exceptions;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class GetCourseMetadataHandler : IRequestHandler<GetCourseMetadataQuery, CourseResponse>
{
    private readonly ICourseRepository _repository;

    public GetCourseMetadataHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<CourseResponse> Handle(GetCourseMetadataQuery request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new CourseNotFoundException(request.CourseId);

        return CourseResponse.FromDomain(course);
    }
}