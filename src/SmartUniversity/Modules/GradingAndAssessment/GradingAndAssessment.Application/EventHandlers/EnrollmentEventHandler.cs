using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Cache;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.EventHandlers;

public class EnrollmentEventHandler
{
    private readonly GradingDbContext _context;

    public EnrollmentEventHandler(GradingDbContext context)
    {
        _context = context;
    }

    public async Task HandleStudentEnrolledAsync(StudentEnrolledEvent notification)
    {
        var enrollmentCache = new EnrollmentCache
        {
            EnrollmentId = notification.EnrollmentId,
            StudentId = notification.StudentId,
            CourseId = notification.CourseId,
            Status = "Enrolled",
            CreatedAt = notification.OccurredOn,
            UpdatedAt = notification.OccurredOn
        };

        _context.EnrollmentCache.Add(enrollmentCache);
        await _context.SaveChangesAsync();
    }

    public async Task HandleStudentDroppedCourseAsync(StudentDroppedCourseEvent notification)
    {
        var enrollmentCache = await _context.EnrollmentCache
            .FirstOrDefaultAsync(e => e.EnrollmentId == notification.EnrollmentId);

        if (enrollmentCache != null)
        {
            enrollmentCache.Status = "Dropped";
            enrollmentCache.UpdatedAt = notification.OccurredOn;
            await _context.SaveChangesAsync();
        }
    }

    public async Task HandleEnrollmentStatusChangedAsync(EnrollmentStatusChangedEvent notification)
    {
        var enrollmentCache = await _context.EnrollmentCache
            .FirstOrDefaultAsync(e => e.EnrollmentId == notification.EnrollmentId);

        if (enrollmentCache != null)
        {
            enrollmentCache.Status = notification.NewStatus.ToString();
            enrollmentCache.UpdatedAt = notification.OccurredOn;
            await _context.SaveChangesAsync();
        }
    }
}