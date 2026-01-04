using SmartUniversity.Modules.Enrollment.Domain.Enums;

namespace SmartUniversity.Modules.Enrollment.Domain.Events;

public record EnrollmentStatusChangedEvent(
    Guid EnrollmentId,
    EnrollmentStatus NewStatus
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
