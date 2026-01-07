using Microsoft.AspNetCore.Http;

namespace SmartUniversity.Modules.Content.Application.DTOs;

public class MaterialDto
{
    public Guid MaterialId { get; set; }
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime UploadDate { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string? Description { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UpdateMaterialRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? LessonId { get; set; }
}

public class CourseMaterialsDashboardDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public List<MaterialDto> Materials { get; set; } = new();
}

// Request class for Upload Endpoint
// public class MaterialUploadRequest
// {
//     public required IFormFile File { get; set; }
//     public string? Title { get; set; }
//     public string? Description { get; set; }
//     public string? LessonId { get; set; } 
//     public string? UploadedByUserId { get; set; }
// }