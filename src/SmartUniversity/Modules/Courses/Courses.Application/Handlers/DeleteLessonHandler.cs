using MediatR;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class DeleteLessonHandler : IRequestHandler<DeleteLessonCommand, Unit>
{
    private readonly ILessonRepository _lessonRepository;

    public DeleteLessonHandler(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<Unit> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        await _lessonRepository.DeleteAsync(lesson);

        return Unit.Value;
    }
}