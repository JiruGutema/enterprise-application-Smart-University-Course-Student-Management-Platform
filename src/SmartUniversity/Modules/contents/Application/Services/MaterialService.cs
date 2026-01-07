using SmartUniversity.Modules.Content.Application.DTOs;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Domain.Entities;
using Microsoft.AspNetCore.Http; 

namespace SmartUniversity.Modules.Content.Application.Services;

public class MaterialService
{
    private readonly IMaterialRepository _repository;

    public MaterialService(IMaterialRepository repository)
    {
        _repository = repository;
    }

    // 1. Upload
    public async Task<Material> UploadAsync(Guid courseId, Guid? lessonId, string title, string fileName, string filePath, string fileType, long size, Guid userId, string? desc)
    {
        var material = new Material(courseId, lessonId, title, fileName, filePath, fileType, size, userId, desc);
        return await _repository.AddAsync(material);
    }

    // 2. Get By ID (Returns DTO)
    public async Task<MaterialDto?> GetMaterialByIdAsync(Guid id)
    {
        var m = await _repository.GetByIdAsync(id);
        return m == null ? null : MapToDto(m);
    }

    // 3. List Materials
    public async Task<PagedResult<MaterialDto>> GetMaterialsForCourseAsync(Guid courseId, Guid? lessonId, string? fileType, string? search, string? sort, int page, int pageSize)
    {
        var (items, total) = await _repository.GetByCourseIdAsync(courseId, lessonId, fileType, search, sort, page, pageSize);
        return new PagedResult<MaterialDto> 
        { 
            Data = items.Select(MapToDto), 
            Total = total, 
            Page = page, 
            PageSize = pageSize 
        };
    }

    // 4. Update Metadata
    public async Task<MaterialDto?> UpdateMetadataAsync(Guid id, string? title, string? desc, Guid? lessonId)
    {
        var m = await _repository.GetByIdAsync(id);
        if (m == null) return null;
        
        m.UpdateMetadata(title, desc, lessonId);
        await _repository.UpdateAsync(m);
        return MapToDto(m);
    }

    // 5. Replace File
    public async Task<MaterialDto?> ReplaceFileAsync(Guid id, IFormFile newFile)
    {
        var m = await _repository.GetByIdAsync(id);
        if (m == null) return null;

        // In a real app, delete the old file from cloud storage here
        if (File.Exists(m.FilePath)) File.Delete(m.FilePath); 

        // Save new file
        var path = Path.Combine("uploads", m.CourseId.ToString());
        Directory.CreateDirectory(path);
        var fullPath = Path.Combine(path, newFile.FileName);
        using (var stream = File.Create(fullPath)) await newFile.CopyToAsync(stream);

        m.ReplaceFile(newFile.FileName, fullPath, newFile.ContentType, newFile.Length);
        await _repository.UpdateAsync(m);
        return MapToDto(m);
    }

    // 6. Delete
    public async Task<bool> DeleteAsync(Guid id)
    {
        var m = await _repository.GetByIdAsync(id);
        if (m == null) return false;
        
        if (File.Exists(m.FilePath)) File.Delete(m.FilePath);
        
        await _repository.DeleteAsync(m);
        return true;
    }

    private static MaterialDto MapToDto(Material m) => new()
    {
        MaterialId = m.Id, CourseId = m.CourseId, LessonId = m.LessonId, Title = m.Title,
        FileName = m.FileName, FilePath = m.FilePath, FileType = m.FileType, SizeInBytes = m.SizeInBytes,
        UploadDate = m.UploadDate, UploadedByUserId = m.UploadedByUserId, Description = m.Description
    };
}