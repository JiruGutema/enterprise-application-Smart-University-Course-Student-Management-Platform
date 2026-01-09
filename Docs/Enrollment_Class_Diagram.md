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

    %% Repository Interface
    class IEnrollmentRepository {
        <<interface>>
        +AddAsync(Enrollment enrollment, CancellationToken ct) Task~Enrollment~
        +GetAsync(Guid id, CancellationToken ct) Task~Enrollment?~
        +ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct) Task~bool~
        +GetByStudentAsync(Guid studentId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +CountByStudentAsync(Guid studentId, string? status, CancellationToken ct) Task~int~
        +GetByCourseAsync(Guid courseId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +CountByCourseAsync(Guid courseId, string? status, CancellationToken ct) Task~int~
        +AdminSearchAsync(Guid? studentId, Guid? courseId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +AdminCountAsync(Guid? studentId, Guid? courseId, string? status, CancellationToken ct) Task~int~
        +SaveAsync(Enrollment enrollment, CancellationToken ct) Task
        +GetByIdAsync(Guid enrollmentId, CancellationToken ct) Task~Enrollment?~
        +GetByStudentIdAsync(Guid studentId, CancellationToken ct) Task~List~Enrollment~~
        +GetByCourseIdAsync(Guid courseId, CancellationToken ct) Task~List~Enrollment~~
    }

    %% Infrastructure - Outbox
    class OutboxMessage {
        +Guid Id
        +string Type
        +byte[] Payload
        +DateTime OccurredOn
        +DateTime? ProcessedOn
        -OutboxMessage()
        +From(IDomainEvent domainEvent) OutboxMessage
        +Deserialize() object?
        +MarkProcessed() void
        +MarkFailed(string reason) void
    }

    class OutboxInterceptor {
        +SavingChangesAsync(DbContextEventData eventData, InterceptionResult~int~ result, CancellationToken cancellationToken) ValueTask~InterceptionResult~int~~
    }

    class EnrollmentOutboxPublisher {
        -EnrollmentDbContext _db
        -IEventBus _eventBus
        +EnrollmentOutboxPublisher(EnrollmentDbContext db, IEventBus eventBus)
        +PublishAsync(CancellationToken ct) Task
    }

    class EnrollmentOutboxPublishJob {
        -EnrollmentOutboxPublisher _publisher
        +EnrollmentOutboxPublishJob(EnrollmentOutboxPublisher publisher)
        +Execute(IJobExecutionContext context) Task
    }

    %% Persistence
    class EnrollmentDbContext {
        +DbSet~Enrollment~ Enrollments
        +DbSet~OutboxMessage~ OutboxMessages
        +EnrollmentDbContext(DbContextOptions~EnrollmentDbContext~ options)
        #OnModelCreating(ModelBuilder modelBuilder) void
    }

    class EnrollmentRepository {
        -EnrollmentDbContext _context
        +EnrollmentRepository(EnrollmentDbContext context)
        +AddAsync(Enrollment enrollment, CancellationToken ct) Task~Enrollment~
        +GetAsync(Guid id, CancellationToken ct) Task~Enrollment?~
        +ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct) Task~bool~
        +GetByStudentAsync(Guid studentId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +CountByStudentAsync(Guid studentId, string? status, CancellationToken ct) Task~int~
        +GetByCourseAsync(Guid courseId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +CountByCourseAsync(Guid courseId, string? status, CancellationToken ct) Task~int~
        +AdminSearchAsync(Guid? studentId, Guid? courseId, string? status, int page, int pageSize, CancellationToken ct) Task~List~Enrollment~~
        +AdminCountAsync(Guid? studentId, Guid? courseId, string? status, CancellationToken ct) Task~int~
        +SaveAsync(Enrollment enrollment, CancellationToken ct) Task
        +GetByIdAsync(Guid enrollmentId, CancellationToken ct) Task~Enrollment?~
        +GetByStudentIdAsync(Guid studentId, CancellationToken ct) Task~List~Enrollment~~
        +GetByCourseIdAsync(Guid courseId, CancellationToken ct) Task~List~Enrollment~~
    }

    class UnitOfWork {
        -EnrollmentDbContext _context
        +UnitOfWork(EnrollmentDbContext context)
        +CommitAsync(CancellationToken ct) Task
    }

    %% Relationships
    Enrollment --|> AggregateRoot : inherits
    Enrollment ..> StudentEnrolledEvent : creates
    Enrollment ..> StudentDroppedCourseEvent : creates
    Enrollment ..> EnrollmentStatusChangedEvent : creates
    
    EnrollmentRepository ..|> IEnrollmentRepository : implements
    EnrollmentRepository --> EnrollmentDbContext : uses
    EnrollmentDbContext --> Enrollment : manages
    EnrollmentDbContext --> OutboxMessage : manages

    OutboxInterceptor --> OutboxMessage : creates
    OutboxInterceptor --> AggregateRoot : intercepts
    EnrollmentOutboxPublisher --> OutboxMessage : processes
    EnrollmentOutboxPublisher --> EnrollmentDbContext : uses
    EnrollmentOutboxPublishJob --> EnrollmentOutboxPublisher : uses

    %% Styling
    classDef aggregate fill:#e1f5fe
    classDef valueObject fill:#f3e5f5
    classDef domainEvent fill:#e8f5e8
    classDef repository fill:#fff3e0
    classDef infrastructure fill:#fce4ec
    classDef exception fill:#ffebee
