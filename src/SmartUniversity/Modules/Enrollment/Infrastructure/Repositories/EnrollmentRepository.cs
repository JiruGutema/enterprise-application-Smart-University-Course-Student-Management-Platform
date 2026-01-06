using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Enrollment.Domain.Enums;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Repositories
{
    public class EnrollmentRepository : SmartUniversity.Modules.Enrollment.Domain.Repositories.IEnrollmentRepository
    {
        private readonly SmartUniversity.Modules.Enrollment.Infrastructure.Persistence.EnrollmentDbContext _context;

        public EnrollmentRepository(SmartUniversity.Modules.Enrollment.Infrastructure.Persistence.EnrollmentDbContext context)
        {
            _context = context;
        }

        public async Task<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment> AddAsync(
            SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment enrollment,
            CancellationToken ct)
        {
            await _context.Enrollments.AddAsync(enrollment, ct);
            await _context.SaveChangesAsync(ct);
            return enrollment;
        }

        public async Task<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment?> GetAsync(
            Guid id,
            CancellationToken ct)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<bool> ExistsAsync(
            Guid studentId,
            Guid courseId,
            CancellationToken ct)
        {
            return await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.Status != SmartUniversity.Modules.Enrollment.Domain.Enums.EnrollmentStatus.Dropped,
                ct);
        }
    }
}
