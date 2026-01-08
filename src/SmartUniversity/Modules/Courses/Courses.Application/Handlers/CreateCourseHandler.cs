using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.ValueObjects;
using SmartUniversity.Modules.Courses.Domain.Repositories;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly ICourseRepository _repository;

    public CreateCourseHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // Convert string → CourseCode
        CourseCode code = CourseCode.Create(request.Code);

        var course = Course.Create(
            request.Title,
            code,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.InstructorId
        );

        // handle prerequisites if provided
        if (request.Prerequisites is not null)
        {
            var prereqCodes = request.Prerequisites
                .Select(CourseCode.Create)
                .ToList();

            course.SetPrerequisites(prereqCodes);
        }

        await _repository.AddAsync(course);

        return course.Id;
    }
}
