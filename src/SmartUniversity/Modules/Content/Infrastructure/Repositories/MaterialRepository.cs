using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Content.Domain.Aggregates;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Infrastructure.Persistence;

namespace SmartUniversity.Modules.Content.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly ContentDbContext _db;

    public MaterialRepository(ContentDbContext db)
    {
        _db = db;
    }

    public async Task<Material> AddAsync(Material material)
    {
        _db.Materials.Add(material);
        await _db.SaveChangesAsync();
        return material;
    }

    public async Task<Material?> GetByIdAsync(Guid materialId)
    {
        return await _db.Materials.FirstOrDefaultAsync(m => m.Id == materialId);
    }

    public async Task<(IEnumerable<Material> Items, int TotalCount)> GetByCourseIdAsync(
        Guid courseId, Guid? lessonId, string? fileType, string? search, string? sort, int page, int pageSize)
    {
        var query = _db.Materials.AsQueryable().Where(m => m.CourseId == courseId);

        if (lessonId.HasValue) 
            query = query.Where(m => m.LessonId == lessonId);
            
        if (!string.IsNullOrWhiteSpace(fileType)) 
            query = query.Where(m => m.FileType.ToLower().Contains(fileType.ToLower()));
            
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(m => m.Title.ToLower().Contains(s) || m.FileName.ToLower().Contains(s));
        }

        var total = await query.CountAsync();

        query = sort?.ToLower() switch {
            "title" => query.OrderBy(m => m.Title),
            "uploaddate_asc" => query.OrderBy(m => m.UploadDate),
            _ => query.OrderByDescending(m => m.UploadDate)
        };

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task UpdateAsync(Material material)
    {
        _db.Materials.Update(material);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Material material)
    {
        _db.Materials.Remove(material);
        await _db.SaveChangesAsync();
    }
}