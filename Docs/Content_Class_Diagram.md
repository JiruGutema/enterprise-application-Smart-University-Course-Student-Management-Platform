```mermaid
classDiagram
    %% --- Shared Kernel ---
    class AggregateRoot {
        <<abstract>>
        -List~object~ _domainEvents
        +IReadOnlyCollection~object~ DomainEvents
        #AddDomainEvent(object domainEvent) void
        +ClearDomainEvents() void
    }

    class DomainException {
        <<abstract>>
        +int StatusCode
    }

    %% --- Aggregate Root: Course ---
    class Course {
        +Guid Id
        +string Title
        +string Description
        +Guid InstructorId
        +decimal Price
        +string? ThumbnailUrl
        +CourseStatus Status
        +DateTime CreatedAt
        +DateTime? PublishedAt
        -List~CourseModule~ _modules
        +IReadOnlyCollection~CourseModule~ Modules
        -Course()
        +Course(Guid id, string title, Guid instructorId, decimal price)
        +UpdateDetails(string title, string description, decimal price) void
        +AddModule(string title, int orderIndex) void
        +Publish() void
        +Archive() void
        +AddMaterialToLesson(Guid moduleId, Guid lessonId, Material material) void
    }

    %% --- Entities ---
    class CourseModule {
        +Guid Id
        +Guid CourseId
        +string Title
        +int OrderIndex
        -List~Lesson~ _lessons
        +IReadOnlyCollection~Lesson~ Lessons
        +AddLesson(string title, string? content) void
    }

    class Lesson {
        +Guid Id
        +Guid ModuleId
        +string Title
        +string? TextContent
        +int OrderIndex
        -List~Material~ _materials
        +IReadOnlyCollection~Material~ Materials
        +AttachMaterial(Material material) void
    }

    class Material {
        +Guid Id
        +Guid LessonId
        +string FileName
        +string FilePath
        +string ContentType
        +long FileSize
        +MaterialType Type
        +DateTime UploadedAt
        +ReplaceFile(string newPath, string newName, long newSize) void
    }

    %% --- Enums ---
    class CourseStatus {
        <<enumeration>>
        Draft = 0
        Published = 1
        Archived = 2
    }

    class MaterialType {
        <<enumeration>>
        Document = 0
        Video = 1
        Audio = 2
    }

    %% --- Domain Events ---
    class CourseCreatedEvent {
        +Guid CourseId
        +string Title
        +Guid InstructorId
        +CourseCreatedEvent(Guid courseId, string title, Guid instructorId)
    }

    class CoursePublishedEvent {
        +Guid CourseId
        +DateTime PublishedAt
        +CoursePublishedEvent(Guid courseId, DateTime publishedAt)
    }

    class ModuleAddedEvent {
        +Guid CourseId
        +Guid ModuleId
        +string ModuleTitle
        +ModuleAddedEvent(Guid courseId, Guid moduleId, string moduleTitle)
    }

    class MaterialUploadedEvent {
        +Guid CourseId
        +Guid MaterialId
        +string FileName
        +MaterialUploadedEvent(Guid courseId, Guid materialId, string fileName)
    }

    %% --- Repository Interface ---
    class ICourseRepository {
        <<interface>>
        +GetByIdAsync(Guid id) Task~Course?~
        +GetByInstructorIdAsync(Guid instructorId) Task~List~Course~~
        +AddAsync(Course course) Task
        +UpdateAsync(Course course) Task
        +DeleteAsync(Guid id) Task
        +ExistsAsync(Guid id) Task~bool~
    }

    class IMaterialRepository {
        <<interface>>
        +GetByIdAsync(Guid id) Task~Material?~
        +AddAsync(Material material) Task
        +DeleteAsync(Guid id) Task
    }

    %% --- Infrastructure - Outbox ---
    class OutboxMessage {
        +Guid Id
        +string Type
        +string Payload
        +DateTime OccurredAt
        +DateTime? ProcessedAt
        +int RetryCount
        +string? Error
        -OutboxMessage()
        +FromEvent(object event)$ OutboxMessage
        +MarkProcessed() void
        +MarkFailed(string error) void
    }

    class ContentOutboxInterceptor {
        +SavingChangesAsync(DbContextEventData eventData, InterceptionResult~int~ result) ValueTask~InterceptionResult~int~~
    }

    class ContentOutboxPublisher {
        -ContentDbContext _db
        -IEventBus _eventBus
        +PublishPendingAsync(CancellationToken ct) Task
    }

    class ContentOutboxPublishJob {
        -ContentOutboxPublisher _publisher
        +Execute(IJobExecutionContext context) Task
    }

    %% --- Persistence ---
    class ContentDbContext {
        +DbSet~Course~ Courses
        +DbSet~CourseModule~ Modules
        +DbSet~Lesson~ Lessons
        +DbSet~Material~ Materials
        +DbSet~OutboxMessage~ OutboxMessages
        +ContentDbContext(DbContextOptions options)
        #OnModelCreating(ModelBuilder modelBuilder) void
    }

    class CourseRepository {
        -ContentDbContext _context
        +CourseRepository(ContentDbContext context)
        +GetByIdAsync(Guid id) Task~Course?~
        +AddAsync(Course course) Task
        +UpdateAsync(Course course) Task
        +DeleteAsync(Guid id) Task
    }

    %% --- Relationships ---
    Course --|> AggregateRoot : inherits
    CourseModule --|> AggregateRoot : inherits_entity
    Lesson --|> AggregateRoot : inherits_entity
    
    Course "1" *-- "many" CourseModule : contains
    CourseModule "1" *-- "many" Lesson : contains
    Lesson "1" *-- "many" Material : contains

    Course --> CourseStatus : uses
    Material --> MaterialType : uses

    Course ..> CourseCreatedEvent : creates
    Course ..> CoursePublishedEvent : creates
    Course ..> ModuleAddedEvent : creates
    Material ..> MaterialUploadedEvent : creates

    CourseRepository ..|> ICourseRepository : implements
    CourseRepository --> ContentDbContext : uses
    ContentDbContext --> Course : manages
    ContentDbContext --> OutboxMessage : manages

    ContentOutboxInterceptor --> OutboxMessage : creates
    ContentOutboxInterceptor --> AggregateRoot : intercepts
    ContentOutboxPublisher --> ContentDbContext : reads
    ContentOutboxPublishJob --> ContentOutboxPublisher : triggers

    %% --- Styling (Exact Identity Match) ---
    classDef aggregate fill:#e1f5fe
    classDef valueObject fill:#f3e5f5
    classDef domainEvent fill:#e8f5e8
    classDef repository fill:#fff3e0
    classDef infrastructure fill:#fce4ec
    classDef exception fill:#ffebee

    %% Apply Styles
    class Course,CourseModule,Lesson,Material aggregate
    class CourseStatus,MaterialType valueObject
    class CourseCreatedEvent,CoursePublishedEvent,ModuleAddedEvent,MaterialUploadedEvent domainEvent
    class ICourseRepository,IMaterialRepository,CourseRepository repository
    class OutboxMessage,ContentOutboxInterceptor,ContentOutboxPublisher,ContentOutboxPublishJob,ContentDbContext infrastructure
    class DomainException exception
```