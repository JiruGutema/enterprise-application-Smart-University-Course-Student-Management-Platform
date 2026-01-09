namespace SmartUniversity.Modules.Content.Domain.Events;

public class MaterialUploadedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid MaterialId { get; }
    public Guid CourseId { get; }
    public Guid UploadedByUserId { get; }
    public string Title { get; }
    public string FileName { get; }

    public MaterialUploadedEvent(Guid materialId, Guid courseId, Guid uploadedByUserId, string title, string fileName)
    {
        MaterialId = materialId;
        CourseId = courseId;
        UploadedByUserId = uploadedByUserId;
        Title = title;
        FileName = fileName;
    }
}