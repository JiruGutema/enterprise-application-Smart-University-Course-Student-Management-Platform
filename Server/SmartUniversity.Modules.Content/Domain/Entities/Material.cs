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
    public DateTime UploadDate { get; private set; }
    public long SizeInBytes { get; private set; }
    public string? Description { get; private set; }

    private Material() { } // EF Core needs this

    public Material(
        Guid courseId,
        Guid? lessonId,
        string title,
        string fileName,
        string filePath,
        string fileType,
        long sizeInBytes,
        Guid uploadedByUserId,
        string? description)
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
    }
}
