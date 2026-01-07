using System.ComponentModel.DataAnnotations.Schema;

namespace SmartUniversity.Modules.Content.Domain.Entities;

[Table("materials", Schema = "content")]
public class Material
{
    [Column("material_id")] public Guid Id { get; private set; }
    [Column("course_id")] public Guid CourseId { get; private set; }
    [Column("lesson_id")] public Guid? LessonId { get; private set; }
    [Column("title")] public string Title { get; private set; } = null!;
    [Column("file_name")] public string FileName { get; private set; } = null!;
    [Column("file_path")] public string FilePath { get; private set; } = null!;
    [Column("file_type")] public string FileType { get; private set; } = null!;
    [Column("uploaded_by_user_id")] public Guid UploadedByUserId { get; private set; }
    [Column("upload_date")] public DateTime UploadDate { get; private set; }
    [Column("size_in_bytes")] public long SizeInBytes { get; private set; }
    [Column("description")] public string? Description { get; private set; }
    
    // Audit fields (optional based on your schema default)
    [Column("created_at")] public DateTime CreatedAt { get; private set; }
    [Column("updated_at")] public DateTime UpdatedAt { get; private set; }

    private Material() { } // Required for EF Core

    public Material(Guid courseId, Guid? lessonId, string title, string fileName, string filePath, string fileType, long sizeInBytes, Guid uploadedByUserId, string? description)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        LessonId = lessonId;
        Title = title;
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        SizeInBytes = sizeInBytes;
        UploadedByUserId = uploadedByUserId;
        Description = description;
        UploadDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business Logic: Update Metadata
    public void UpdateMetadata(string? title, string? description, Guid? lessonId)
    {
        if (!string.IsNullOrWhiteSpace(title)) Title = title;
        if (description != null) Description = description;
        if (lessonId.HasValue) LessonId = lessonId;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business Logic: Replace File
    public void ReplaceFile(string fileName, string filePath, string fileType, long sizeInBytes)
    {
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        SizeInBytes = sizeInBytes;
        UploadDate = DateTime.UtcNow; // Reset upload date to now
        UpdatedAt = DateTime.UtcNow;
    }
}