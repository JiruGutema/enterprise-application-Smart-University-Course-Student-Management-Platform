using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;

namespace SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid assignmentId);
    Task<List<Assignment>> GetByCourseIdAsync(Guid courseId);
    Task AddAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task DeleteAsync(Guid assignmentId);
}

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid gradeId);
    Task<Grade?> GetByEnrollmentAndAssignmentAsync(Guid enrollmentId, Guid assignmentId);
    Task<List<Grade>> GetByEnrollmentIdAsync(Guid enrollmentId);
    Task<List<Grade>> GetByAssignmentIdAsync(Guid assignmentId);
    Task AddAsync(Grade grade);
    Task UpdateAsync(Grade grade);
}