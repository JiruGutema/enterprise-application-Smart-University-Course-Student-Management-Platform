using System.Text.Json;

namespace SmartUniversity.Modules.Identity.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = default!;
    public string Payload { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage FromEvent(object @event)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = @event.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(@event),
            OccurredAt = DateTime.UtcNow,
        };
    }

    public void MarkProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        RetryCount++;
        Error = error;
    }

    public object? Deserialize()
    {
        var type = System.Type.GetType(Type);
        if (type == null)
        {
             // Fallback or specific assembly loading logic might be needed if Type is just Name not AssemblyQualifiedName.
             // Given Line 22: Type = @event.GetType().Name in FromEvent,
             // We only stored the Name (e.g., "UserCreatedEvent"). This is problematic for deserialization without knowing the assembly/namespace.
             // We should probably fix FromEvent to store AssemblyQualifiedName or at least FullName.
             return null; 
        }
        return JsonSerializer.Deserialize(Payload, type);
    }
}
