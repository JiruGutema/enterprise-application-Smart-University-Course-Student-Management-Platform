namespace SmartUniversity.Modules.Content.Domain.Entities;

public class Material
{
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid? LessonId { get; private set; }
    public string Title { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string FileType { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public long SizeInBytes { get; private set; }
    public string? Description { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Material() { } // Required by EF Core

    public Material(
        Guid id,
        Guid courseId,
        Guid? lessonId,
        string title,
        string fileName,
        string filePath,
        string fileType,
        Guid uploadedByUserId,
        long sizeInBytes,
        string? description)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("CourseId is required");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required");

        if (string.IsNullOrWhiteSpace(fileType))
            throw new ArgumentException("File type is required");

        if (uploadedByUserId == Guid.Empty)
            throw new ArgumentException("UploadedByUserId is required");

        if (sizeInBytes <= 0)
            throw new ArgumentException("File size must be greater than zero");

        Id = id;
        CourseId = courseId;
        LessonId = lessonId;
        Title = string.IsNullOrWhiteSpace(title) ? fileName : title;
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        UploadedByUserId = uploadedByUserId;
        SizeInBytes = sizeInBytes;
        Description = description;
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void UpdateMetadata(string title, string? description, Guid? lessonId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        Title = title;
        Description = description;
        LessonId = lessonId;
    }

    public void ReplaceFile(string newFilePath, long newSizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(newFilePath))
            throw new ArgumentException("File path is required");

        if (newSizeInBytes <= 0)
            throw new ArgumentException("File size must be greater than zero");

        FilePath = newFilePath;
        SizeInBytes = newSizeInBytes;
        UploadedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
