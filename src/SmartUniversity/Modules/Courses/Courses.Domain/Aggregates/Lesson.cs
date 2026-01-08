using SmartUniversity.Modules.Courses.Domain.Common;

namespace SmartUniversity.Modules.Courses.Domain.Aggregates;

public class Lesson : Entity
{
    public Guid Id { get; private set; }
    public Guid ModuleId { get; private set; }
    public string Name { get; private set; }
    public string? Content { get; private set; }
    public int Order { get; private set; }

    private Lesson() { } // For EF Core

    private Lesson(Guid id, Guid moduleId, string name, string? content, int order)
    {
        Id = id;
        ModuleId = moduleId;
        Name = name;
        Content = content;
        Order = order;
    }

    public static Lesson Create(Guid moduleId, string name, string? content, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Lesson name is required.");

        return new Lesson(Guid.NewGuid(), moduleId, name.Trim(), content, order);
    }

    public void Update(string? name, string? content, int? order)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();

        if (!string.IsNullOrWhiteSpace(content))
            Content = content;

        if (order.HasValue)
            Order = order.Value;
    }
}