using System.ComponentModel.DataAnnotations.Schema;

namespace SmartUniversity.Modules.Content.Infrastructure.Outbox;

[Table("outbox_messages", Schema = "content")]
public class OutboxMessage
{
    [Column("id")] public Guid Id { get; set; }
    [Column("type")] public string Type { get; set; } = string.Empty;
    [Column("content")] public string Content { get; set; } = string.Empty;
    [Column("occurred_on")] public DateTime OccurredOn { get; set; }
    [Column("processed_on")] public DateTime? ProcessedOn { get; set; }
    [Column("error")] public string? Error { get; set; }
}