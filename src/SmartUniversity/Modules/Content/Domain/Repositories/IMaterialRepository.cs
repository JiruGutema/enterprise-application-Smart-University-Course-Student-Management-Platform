// using SmartUniversity.Modules.Content.Domain.Aggregates;

// namespace SmartUniversity.Modules.Content.Domain.Repositories;

// public interface IMaterialRepository
// {
//     Task<Material> AddAsync(Material material);
//     Task<Material?> GetByIdAsync(Guid materialId);
    
//     // Supports filtering by Lesson, Type, and Search + Pagination
//     Task<(IEnumerable<Material> Items, int TotalCount)> GetByCourseIdAsync(
//         Guid courseId, 
//         Guid? lessonId, 
//         string? fileType, 
//         string? search, 
//         string? sort, 
//         int page, 
//         int pageSize);
        
//     Task UpdateAsync(Material material);
//     Task DeleteAsync(Material material);
// }
/// <summary>
/// Repository contract for managing Material entities.
/// </summary>
public interface IMaterialRepository
{
    /// <summary>
    /// Adds a new material to the data store.
    /// </summary>
    Task<Material> AddAsync(Material material);

    /// <summary>
    /// Gets a material by its unique identifier.
    /// </summary>
    Task<Material?> GetByIdAsync(Guid materialId);

    /// <summary>
    /// Gets materials for a course with optional filters and pagination.
    /// </summary>
    Task<(IEnumerable<Material> Items, int TotalCount)> GetByCourseIdAsync(
        Guid courseId,
        Guid? lessonId,
        string? fileType,
        string? search,
        string? sort,
        int page,
        int pageSize);

    /// <summary>
    /// Updates an existing material.
    /// </summary>
    Task UpdateAsync(Material material);

    /// <summary>
    /// Deletes a material.
    /// </summary>
    Task DeleteAsync(Material material);
}
