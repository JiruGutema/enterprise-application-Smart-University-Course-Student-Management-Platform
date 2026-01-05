using SmartUniversity.Modules.Content.Domain.Entities;
using SmartUniversity.Modules.Content.Domain.Repositories;

namespace SmartUniversity.Modules.Content.Infrastructure.Persistence;

public class MaterialRepository : IMaterialRepository
{
    private readonly ContentDbContext _db;

    public MaterialRepository(ContentDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Material material)
    {
        _db.Materials.Add(material);
        await _db.SaveChangesAsync();
    }
}
