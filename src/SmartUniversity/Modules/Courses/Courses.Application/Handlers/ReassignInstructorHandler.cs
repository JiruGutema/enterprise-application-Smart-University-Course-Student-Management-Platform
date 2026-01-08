using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Exceptions;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class ReassignInstructorHandler : IRequestHandler<ReassignInstructorCommand>
{
    private readonly ICourseRepository _repository;

    public ReassignInstructorHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(ReassignInstructorCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new CourseNotFoundException(request.CourseId);

        course.ChangeInstructor(request.NewInstructorId);
        await _repository.UpdateAsync(course);
        return Unit.Value;
    }
}
