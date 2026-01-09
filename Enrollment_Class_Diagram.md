classDiagram
    %% Shared Kernel
    class AggregateRoot {
        <<abstract>>
        -List~object~ _domainEvents
        +IReadOnlyCollection~object~ DomainEvents
        #AddDomainEvent(object domainEvent) void
        +ClearDomainEvents() void
    }

    %% Aggregate Root
    class Enrollment {
        +Guid Id
        +Guid StudentId
        +Guid CourseId
        +EnrollmentStatus Status
        +DateTime EnrolledAt
        -Enrollment()
        +Enrollment(Guid id, Guid studentId, Guid courseId, EnrollmentStatus status, DateTime enrolledAt)
        +ChangeStatus(EnrollmentStatus newStatus) void
    }

    %% Value Objects / Enums
    class EnrollmentStatus {
        <<enumeration>>
        Pending = 0
        Active = 1
        Completed = 2
        Dropped = 3
    }

    %% Domain Events
    class StudentEnrolledEvent {
        +Guid EnrollmentId
        +Guid StudentId
        +Guid CourseId
        +DateTime EnrolledAt
        +StudentEnrolledEvent(Guid enrollmentId, Guid studentId, Guid courseId, DateTime enrolledAt)
    }

    class StudentDroppedCourseEvent {
        +Guid EnrollmentId
        +Guid StudentId
        +Guid CourseId
        +DateTime DroppedAt
        +StudentDroppedCourseEvent(Guid enrollmentId, Guid studentId, Guid courseId, DateTime droppedAt)
    }

    class EnrollmentStatusChangedEvent {
        +Guid EnrollmentId
        +Guid StudentId
        +Guid CourseId
        +EnrollmentStatus OldStatus
        +EnrollmentStatus NewStatus
        +DateTime ChangedAt
        +EnrollmentStatusChangedEvent(Guid enrollmentId, Guid studentId, Guid courseId, EnrollmentStatus oldStatus, EnrollmentStatus newStatus, DateTime changedAt)
    }
