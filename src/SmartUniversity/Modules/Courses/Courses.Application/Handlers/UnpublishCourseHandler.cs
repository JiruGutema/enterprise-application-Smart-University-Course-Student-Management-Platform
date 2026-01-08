using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Exceptions;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class UnpublishCourseHandler : IRequestHandler<UnpublishCourseCommand>
{
    private readonly ICourseRepository _repository;

    public UnpublishCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UnpublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId) ?? throw new CourseNotFoundException(request.CourseId);
        course.Unpublish();
        await _repository.UpdateAsync(course);
        return Unit.Value;
    }
}
