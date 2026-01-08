using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Exceptions;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class PublishCourseHandler : IRequestHandler<PublishCourseCommand>
{
    private readonly ICourseRepository _repository;

    public PublishCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new CourseNotFoundException(request.CourseId);

        course.Publish();
        await _repository.UpdateAsync(course);
        return Unit.Value;
    }
}
