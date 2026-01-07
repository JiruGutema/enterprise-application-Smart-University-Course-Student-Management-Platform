using System.Text.Json;
using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }

    public IDomainEvent? Deserialize()
    {
        var type = System.Type.GetType(Type);
        if (type == null) return null;
        return JsonSerializer.Deserialize(Data, type) as IDomainEvent;
    }

    public void MarkProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }
}