using MediatR;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using SmartUniversity.Modules.Courses.Application.Commands;

namespace SmartUniversity.Modules.Courses.Application.Handlers;

public class UpdateLessonHandler : IRequestHandler<UpdateLessonCommand, Unit>
{
    private readonly ILessonRepository _lessonRepository;

    public UpdateLessonHandler(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<Unit> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        lesson.Update(request.Name, request.Content, request.Order);
        await _lessonRepository.UpdateAsync(lesson);

        return Unit.Value;
    }
}