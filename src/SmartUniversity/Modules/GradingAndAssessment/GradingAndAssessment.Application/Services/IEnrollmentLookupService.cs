namespace SmartUniversity.Modules.GradingAndAssessment.Application.Services;

public interface IEnrollmentLookupService
{
    Task<Guid?> GetEnrollmentIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<string?> GetStudentNameAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<string?> GetCourseTitleAsync(Guid courseId, CancellationToken cancellationToken = default);
}