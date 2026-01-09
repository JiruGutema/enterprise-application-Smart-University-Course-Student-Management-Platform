using System.Text.Json;

namespace SmartUniversity.Modules.Notification.Infrastructure.Outbox;

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

    public bool ShouldRetry()
    {
        return RetryCount < 5 && 
               DateTime.UtcNow > OccurredAt.AddMinutes(Math.Pow(2, RetryCount));
    }

    public bool HasExceededMaxRetries()
    {
        return RetryCount >= 5;
    }

    public void MarkAsDeadLetter(string reason)
    {
        Error = $"Dead Letter: {reason}";
    }

    public object? Deserialize()
    {
        var type = System.Type.GetType(Type);
        if (type == null)
        {
            return null; 
        }
        return JsonSerializer.Deserialize(Payload, type);
    }
}