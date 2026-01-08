using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Cache;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.EventHandlers;

public class CourseEventHandler
{
    private readonly GradingDbContext _context;

    public CourseEventHandler(GradingDbContext context)
    {
        _context = context;
    }

    public async Task HandleCourseCreatedAsync(CourseCreatedEvent notification)
    {
        var courseCache = new CourseCache
        {
            CourseId = notification.CourseId,
            Title = notification.Title,
            Code = notification.Code,
            InstructorId = notification.InstructorId,
            CreatedAt = notification.OccurredOn,
            UpdatedAt = notification.OccurredOn
        };

        _context.CourseCache.Add(courseCache);
        await _context.SaveChangesAsync();
    }
}