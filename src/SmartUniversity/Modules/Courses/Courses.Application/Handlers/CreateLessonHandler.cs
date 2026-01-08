using MediatR;
using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, Guid>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IModuleRepository _moduleRepository;

    public CreateLessonHandler(ILessonRepository lessonRepository, IModuleRepository moduleRepository)
    {
        _lessonRepository = lessonRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<Guid> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        // Verify module exists
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        var lesson = Lesson.Create(request.ModuleId, request.Name, request.Content, request.Order);
        await _lessonRepository.AddAsync(lesson);

        return lesson.Id;
    }
}