using SmartUniversity.Modules.Enrollment.Application;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly EnrollmentDbContext _context;

    public UnitOfWork(EnrollmentDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}
