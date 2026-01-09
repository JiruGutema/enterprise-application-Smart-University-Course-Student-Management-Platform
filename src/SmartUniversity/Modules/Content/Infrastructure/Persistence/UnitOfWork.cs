using SmartUniversity.Modules.Content.Application.Interfaces;

namespace SmartUniversity.Modules.Content.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContentDbContext _context;

    public UnitOfWork(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}