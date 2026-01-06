namespace SmartUniversity.Modules.Enrollment.Domain.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment> AddAsync(
            SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment enrollment, 
            CancellationToken ct);

        Task<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment?> GetAsync(
            Guid id, 
            CancellationToken ct);

        Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct);

    }
}
