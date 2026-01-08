using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Exceptions;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Domain.ValueObjects;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand>
{
    private readonly ICourseRepository _repository;

    public UpdateCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new CourseNotFoundException(request.CourseId);

        // Convert string[] to List<CourseCode> safely
        List<CourseCode>? prereqCodes = null;
        if (request.Prerequisites != null && request.Prerequisites.Any())
        {
            prereqCodes = request.Prerequisites
                .Select(CourseCode.Create) // convert string -> CourseCode
                .ToList();
        }

        course.Update(
            request.Title,
            request.Code,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.InstructorId,
            prereqCodes
        );

        await _repository.UpdateAsync(course);
        return Unit.Value;
    }
}
