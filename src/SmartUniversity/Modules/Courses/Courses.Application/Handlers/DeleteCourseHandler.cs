using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Exceptions;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseRepository _repository;

    public DeleteCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new CourseNotFoundException(request.CourseId);

        // optional: check if enrollments exist, throw InvalidCourseOperationException
        await _repository.DeleteAsync(course);
        return Unit.Value;
    }
}
