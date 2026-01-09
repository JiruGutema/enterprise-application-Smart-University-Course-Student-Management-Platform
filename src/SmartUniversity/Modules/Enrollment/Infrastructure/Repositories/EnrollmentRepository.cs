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

        public async Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByStudentAsync(Guid studentId, string? status, int page, int pageSize, CancellationToken ct)
{
    var query = _context.Enrollments.Where(e => e.StudentId == studentId);

    if (!string.IsNullOrEmpty(status) && Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
        query = query.Where(e => e.Status == statusEnum);

    return await query
        .OrderByDescending(e => e.EnrolledAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
}

public async Task<int> CountByStudentAsync(Guid studentId, string? status, CancellationToken ct)
{
    var query = _context.Enrollments.Where(e => e.StudentId == studentId);

    if (!string.IsNullOrEmpty(status) && Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
        query = query.Where(e => e.Status == statusEnum);

    return await query.CountAsync(ct);
}

public async Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByCourseAsync(
    Guid courseId,
    string? status,
    int page,
    int pageSize,
    CancellationToken ct)
{
    var query = _context.Enrollments
        .Where(e => e.CourseId == courseId);

    if (!string.IsNullOrEmpty(status) &&
        Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
    {
        query = query.Where(e => e.Status == statusEnum);
    }

    return await query
        .OrderByDescending(e => e.EnrolledAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
}

public async Task<int> CountByCourseAsync(
    Guid courseId,
    string? status,
    CancellationToken ct)
{
    var query = _context.Enrollments
        .Where(e => e.CourseId == courseId);

    if (!string.IsNullOrEmpty(status) &&
        Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
    {
        query = query.Where(e => e.Status == statusEnum);
    }

    return await query.CountAsync(ct);
}

public async Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> AdminSearchAsync(
    Guid? studentId,
    Guid? courseId,
    string? status,
    int page,
    int pageSize,
    CancellationToken ct)
{
    var query = _context.Enrollments.AsQueryable();

    if (studentId.HasValue)
        query = query.Where(e => e.StudentId == studentId.Value);

    if (courseId.HasValue)
        query = query.Where(e => e.CourseId == courseId.Value);

    if (!string.IsNullOrEmpty(status) &&
        Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
    {
        query = query.Where(e => e.Status == statusEnum);
    }

    return await query
        .OrderByDescending(e => e.EnrolledAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
}

public async Task<int> AdminCountAsync(
    Guid? studentId,
    Guid? courseId,
    string? status,
    CancellationToken ct)
{
    var query = _context.Enrollments.AsQueryable();

    if (studentId.HasValue)
        query = query.Where(e => e.StudentId == studentId.Value);

    if (courseId.HasValue)
        query = query.Where(e => e.CourseId == courseId.Value);

    if (!string.IsNullOrEmpty(status) &&
        Enum.TryParse<EnrollmentStatus>(status, true, out var statusEnum))
    {
        query = query.Where(e => e.Status == statusEnum);
    }

    return await query.CountAsync(ct);
}

public async Task SaveAsync(SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment enrollment, CancellationToken ct)
{
    _context.Enrollments.Update(enrollment);
    await _context.SaveChangesAsync(ct);
}

 public async Task<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment?> GetByIdAsync(
        Guid enrollmentId,
        CancellationToken ct)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == enrollmentId, ct);
    }


public async Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByStudentIdAsync(
    Guid studentId,
    CancellationToken ct)
{
    return await _context.Enrollments
        .Where(e => e.StudentId == studentId)
        .ToListAsync(ct);
}

public async Task<List<SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment>> GetByCourseIdAsync(
    Guid courseId,
    CancellationToken ct)
{
    return await _context.Enrollments
        .Where(e => e.CourseId == courseId)
        .ToListAsync(ct);
}


    }
}
