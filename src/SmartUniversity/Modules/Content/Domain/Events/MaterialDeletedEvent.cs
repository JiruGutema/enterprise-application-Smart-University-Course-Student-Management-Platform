namespace SmartUniversity.Modules.Content.Domain.Events;

public class MaterialDeletedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid MaterialId { get; }
    public Guid CourseId { get; }
    public string Title { get; }

    public MaterialDeletedEvent(Guid materialId, Guid courseId, string title)
    {
        MaterialId = materialId;
        CourseId = courseId;
        Title = title;
    }
}