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

        Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByStudentAsync(Guid studentId, string? status, int page, int pageSize, CancellationToken ct);
Task<int> CountByStudentAsync(Guid studentId, string? status, CancellationToken ct);


        Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByCourseAsync(
            Guid courseId,
            string? status,
            int page,
            int pageSize,
            CancellationToken ct
        );

        Task<int> CountByCourseAsync(
            Guid courseId,
            string? status,
            CancellationToken ct
        );

Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> AdminSearchAsync(
    Guid? studentId,
    Guid? courseId,
    string? status,
    int page,
    int pageSize,
    CancellationToken ct);

Task<int> AdminCountAsync(
    Guid? studentId,
    Guid? courseId,
    string? status,
    CancellationToken ct);


    }
}
