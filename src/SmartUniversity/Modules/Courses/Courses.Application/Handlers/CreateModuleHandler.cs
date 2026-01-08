using MediatR;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class CreateModuleHandler : IRequestHandler<CreateModuleCommand, Guid>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateModuleHandler(IModuleRepository moduleRepository, ICourseRepository courseRepository)
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Guid> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        var module = Module.Create(request.CourseId, request.Name, request.Description, request.Order);
        await _moduleRepository.AddAsync(module);

        return module.Id;
    }
}