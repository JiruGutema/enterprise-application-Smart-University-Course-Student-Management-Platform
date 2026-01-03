using SmartUniversity.Modules.Enrollment.Domain.Entities;
using SmartUniversity.Modules.Enrollment.Domain.Enums;
using SmartUniversity.Modules.Enrollment.Domain.Events;

namespace SmartUniversity.Modules.Enrollment.Domain.Aggregates;

public class Enrollment
{
    private readonly List<Attendance> _attendances = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public DateTime EnrolledAt { get; private set; }

    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Enrollment() { }

    public Enrollment(Guid studentId, Guid courseId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        CourseId = courseId;
        Status = EnrollmentStatus.Enrolled;
        ProgressPercentage = 0;
        EnrolledAt = DateTime.UtcNow;

        AddEvent(new StudentEnrolledEvent(Id, StudentId, CourseId));
    }

    public void Drop()
    {
        if (Status == EnrollmentStatus.Dropped)
            throw new InvalidOperationException("Enrollment already dropped.");

        Status = EnrollmentStatus.Dropped;
        AddEvent(new StudentDroppedCourseEvent(Id, StudentId, CourseId));
    }

    public void ChangeStatus(EnrollmentStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        AddEvent(new EnrollmentStatusChangedEvent(Id, newStatus));
    }

    private void AddEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
