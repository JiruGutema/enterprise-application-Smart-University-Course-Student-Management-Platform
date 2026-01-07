using SmartUniversity.Modules.Content.Domain.Entities;

namespace SmartUniversity.Modules.Content.Domain.Repositories;

public interface IMaterialRepository
{
    Task<Material> AddAsync(Material material);
    Task<Material?> GetByIdAsync(Guid materialId);
    
    // Supports filtering by Lesson, Type, and Search + Pagination
    Task<(IEnumerable<Material> Items, int TotalCount)> GetByCourseIdAsync(
        Guid courseId, 
        Guid? lessonId, 
        string? fileType, 
        string? search, 
        string? sort, 
        int page, 
        int pageSize);
        
    Task UpdateAsync(Material material);
    Task DeleteAsync(Material material);
}