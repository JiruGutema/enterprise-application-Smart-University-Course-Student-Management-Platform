using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Enrollment.Domain.Aggregates; // <-- Enrollment class is here
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly EnrollmentDbContext _context;

        public EnrollmentRepository(EnrollmentDbContext context)
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
            return await _context.Enrollments.FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
