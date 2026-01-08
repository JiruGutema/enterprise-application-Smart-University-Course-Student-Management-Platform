using SmartUniversity.Modules.Courses.Domain.Common;

namespace SmartUniversity.Modules.Courses.Domain.Aggregates;

public class Module : Entity
{
    private readonly List<Lesson> _lessons = new();

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    private Module() { } // For EF Core

    private Module(Guid id, Guid courseId, string name, string? description, int order)
    {
        Id = id;
        CourseId = courseId;
        Name = name;
        Description = description;
        Order = order;
    }

    public static Module Create(Guid courseId, string name, string? description, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Module name is required.");

        return new Module(Guid.NewGuid(), courseId, name.Trim(), description, order);
    }

    public void Update(string? name, string? description, int? order)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (order.HasValue)
            Order = order.Value;
    }

    public void AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
    }

    public void RemoveLesson(Lesson lesson)
    {
        _lessons.Remove(lesson);
    }
}