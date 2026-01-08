using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Services;

public class EnrollmentLookupService : IEnrollmentLookupService
{
    private readonly GradingDbContext _context;

    public EnrollmentLookupService(GradingDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> GetEnrollmentIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.EnrollmentCache
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Status == "Enrolled", 
                cancellationToken);
        
        return enrollment?.EnrollmentId;
    }

    public async Task<string?> GetStudentNameAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await _context.StudentCache
            .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);
        
        return student?.FullName;
    }

    public async Task<string?> GetCourseTitleAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await _context.CourseCache
            .FirstOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);
        
        return course?.Title;
    }
}