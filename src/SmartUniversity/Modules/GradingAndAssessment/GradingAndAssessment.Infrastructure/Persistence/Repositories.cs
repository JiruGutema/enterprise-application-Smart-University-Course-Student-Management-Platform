using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly GradingDbContext _context;

    public AssignmentRepository(GradingDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid assignmentId)
    {
        return await _context.Assignments.FindAsync(assignmentId);
    }

    public async Task<List<Assignment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.DueDate)
            .ToListAsync();
    }

    public async Task AddAsync(Assignment assignment)
    {
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid assignmentId)
    {
        var assignment = await GetByIdAsync(assignmentId);
        if (assignment != null)
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}

public class GradeRepository : IGradeRepository
{
    private readonly GradingDbContext _context;

    public GradeRepository(GradingDbContext context)
    {
        _context = context;
    }

    public async Task<Grade?> GetByIdAsync(Guid gradeId)
    {
        return await _context.Grades.FindAsync(gradeId);
    }

    public async Task<Grade?> GetByEnrollmentAndAssignmentAsync(Guid enrollmentId, Guid assignmentId)
    {
        return await _context.Grades
            .FirstOrDefaultAsync(g => g.EnrollmentId == enrollmentId && g.AssignmentId == assignmentId);
    }

    public async Task<List<Grade>> GetByEnrollmentIdAsync(Guid enrollmentId)
    {
        return await _context.Grades
            .Where(g => g.EnrollmentId == enrollmentId)
            .ToListAsync();
    }

    public async Task<List<Grade>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await _context.Grades
            .Where(g => g.AssignmentId == assignmentId)
            .ToListAsync();
    }

    public async Task AddAsync(Grade grade)
    {
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Grade grade)
    {
        _context.Grades.Update(grade);
        await _context.SaveChangesAsync();
    }
}