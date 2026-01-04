namespace SmartUniversity.Modules.Enrollment.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
