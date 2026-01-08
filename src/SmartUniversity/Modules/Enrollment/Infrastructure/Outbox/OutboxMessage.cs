using System;
using System.Text;
using System.Text.Json;
using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Outbox
{
    public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = null!;
    public byte[] Payload { get; set; } = null!;
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedOn { get; set; }

    public static OutboxMessage From(IDomainEvent domainEvent)
    {
        return new OutboxMessage
        {
            Type = domainEvent.GetType().Name,
            Payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(domainEvent, domainEvent.GetType())),
            OccurredOn = domainEvent.OccurredOn
        };
    }

    public object? Deserialize()
    {
        return Type switch
        {
            nameof(StudentEnrolledEvent) => JsonSerializer.Deserialize<StudentEnrolledEvent>(Encoding.UTF8.GetString(Payload)),
            nameof(StudentDroppedCourseEvent) => JsonSerializer.Deserialize<StudentDroppedCourseEvent>(Encoding.UTF8.GetString(Payload)),
            nameof(EnrollmentStatusChangedEvent) => JsonSerializer.Deserialize<EnrollmentStatusChangedEvent>(Encoding.UTF8.GetString(Payload)),
            _ => null
        };
    }

    public void MarkProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        ProcessedOn = null;
    }
}

}
