```mermaid
classDiagram
    %% ==========================================
    %% 1. SHARED KERNEL (Base Classes)
    %% ==========================================
    class AggregateRoot {
        <<abstract>>
        -List~object~ _domainEvents
        +IReadOnlyCollection~object~ DomainEvents
        #AddDomainEvent(object domainEvent) void
        +ClearDomainEvents() void
        +CheckRule(IBusinessRule rule) void
    }

    class Entity {
        <<abstract>>
        +Guid Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        #Entity()
    }

    class DomainException {
        <<abstract>>
        +int StatusCode
        #DomainException(string message)
    }

    class CourseDomainException {
        +CourseDomainException(string message)
        +CourseDomainException(string message, Exception inner)
    }

    %% ==========================================
    %% 2. DOMAIN LAYER (Aggregates & Entities)
    %% ==========================================
    class Course {
        +string Title
        +string Description
        +string? ThumbnailUrl
        +Guid InstructorId
        +decimal Price
        +string Currency
        +CourseLevel Level
        +string Language
        +CourseStatus Status
        +DateTime? PublishedAt
        -List~CourseModule~ _modules
        +IReadOnlyCollection~CourseModule~ Modules
        -Course()
        +Create(Guid id, string title, Guid instructorId, decimal price, string currency) static~Course~
        +UpdateInfo(string title, string description, string thumbnail) void
        +SetPricing(decimal price, string currency) void
        +AddModule(string title, int orderIndex) void
        +UpdateModule(Guid moduleId, string newTitle) void
        +RemoveModule(Guid moduleId) void
        +AddLessonToModule(Guid moduleId, string title, string? content) void
        +Publish() void
        +Archive() void
        +SubmitForReview() void
    }

    class CourseModule {
        +Guid CourseId
        +string Title
        +int OrderIndex
        -List~Lesson~ _lessons
        +IReadOnlyCollection~Lesson~ Lessons
        -CourseModule()
        +CourseModule(Guid courseId, string title, int orderIndex)
        +UpdateTitle(string newTitle) void
        +Reorder(int newIndex) void
        +AddLesson(string title, string? content, int orderIndex) void
        +RemoveLesson(Guid lessonId) void
    }

    class Lesson {
        +Guid ModuleId
        +string Title
        +string? TextContent
        +string? VideoUrl
        +bool IsPreviewable
        +int OrderIndex
        -List~Material~ _materials
        +IReadOnlyCollection~Material~ Materials
        -Lesson()
        +Lesson(Guid moduleId, string title, int orderIndex)
        +UpdateContent(string title, string? text, string? videoUrl) void
        +TogglePreview(bool isPreviewable) void
        +AttachMaterial(string fileName, string filePath, long size, MaterialType type) void
        +RemoveMaterial(Guid materialId) void
    }

    class Material {
        +Guid LessonId
        +string FileName
        +string FilePath
        +string ContentType
        +long FileSize
        +MaterialType Type
        +DateTime UploadedAt
        -Material()
        +Material(Guid lessonId, string fileName, string path, long size, MaterialType type)
        +UpdateFile(string newPath, string newName, long newSize) void
    }

    %% ==========================================
    %% 3. ENUMS & VALUE OBJECTS
    %% ==========================================
    class CourseStatus {
        <<enumeration>>
        Draft = 0
        InReview = 1
        Published = 2
        Archived = 3
    }

    class CourseLevel {
        <<enumeration>>
        Beginner = 0
        Intermediate = 1
        Advanced = 2
    }

    class MaterialType {
        <<enumeration>>
        PDF = 0
        Video = 1
        Audio = 2
        SourceCode = 3
    }

    %% ==========================================
    %% 4. DOMAIN EVENTS
    %% ==========================================
    class CourseCreatedEvent {
        +Guid CourseId
        +string Title
        +Guid InstructorId
        +DateTime OccurredOn
    }

    class CoursePublishedEvent {
        +Guid CourseId
        +DateTime PublishedAt
        +DateTime OccurredOn
    }

    class ModuleAddedEvent {
        +Guid CourseId
        +Guid ModuleId
        +string Title
    }

    class LessonContentUpdatedEvent {
        +Guid CourseId
        +Guid LessonId
        +bool HasVideo
    }

    class MaterialUploadedEvent {
        +Guid CourseId
        +Guid LessonId
        +Guid MaterialId
        +string FileName
        +long Size
    }

    %% ==========================================
    %% 5. REPOSITORY INTERFACES
    %% ==========================================
    class ICourseRepository {
        <<interface>>
        +GetByIdAsync(Guid id) Task~Course?~
        +GetByInstructorIdAsync(Guid instructorId) Task~List~Course~~
        +ExistsAsync(Guid id) Task~bool~
        +AddAsync(Course course) Task
        +UpdateAsync(Course course) Task
        +DeleteAsync(Guid id) Task
        +GetCourseWithModulesAsync(Guid id) Task~Course?~
    }

    class IMaterialRepository {
        <<interface>>
        +GetByIdAsync(Guid id) Task~Material?~
        +AddAsync(Material material) Task
        +RemoveAsync(Material material) Task
    }

    %% ==========================================
    %% 6. INFRASTRUCTURE (Persistence & Outbox)
    %% ==========================================
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
    }

    class OutboxMessage {
        +Guid Id
        +string Type
        +string Payload
        +DateTime OccurredAt
        +DateTime? ProcessedAt
        +string? Error
        +int RetryCount
        -OutboxMessage()
        +FromEvent(object domainEvent)$ OutboxMessage
        +MarkAsProcessed() void
        +MarkAsFailed(string error) void
    }

    class ContentOutboxInterceptor {
        +SavingChangesAsync(DbContextEventData eventData, InterceptionResult~int~ result) ValueTask~InterceptionResult~int~~
        -ConvertEventsToOutboxMessages(DbContext context) void
    }

    class ContentOutboxPublisher {
        -ContentDbContext _db
        -IEventBus _bus
        +PublishPendingAsync(CancellationToken ct) Task
    }

    class ContentOutboxPublishJob {
        -ContentOutboxPublisher _publisher
        +Execute(IJobExecutionContext context) Task
    }

    %% ==========================================
    %% 7. RELATIONSHIPS
    %% ==========================================
    
    %% Inheritance
    Course --|> AggregateRoot : inherits
    CourseModule --|> Entity : inherits
    Lesson --|> Entity : inherits
    Material --|> Entity : inherits
    CourseDomainException --|> DomainException : inherits

    %% Composition (The Aggregate Structure)
    Course "1" *-- "0..*" CourseModule : has modules
    CourseModule "1" *-- "0..*" Lesson : has lessons
    Lesson "1" *-- "0..*" Material : has attachments

    %% Enum Usage
    Course --> CourseStatus : uses
    Course --> CourseLevel : uses
    Material --> MaterialType : uses

    %% Events Generation
    Course ..> CourseCreatedEvent : raises
    Course ..> CoursePublishedEvent : raises
    Course ..> ModuleAddedEvent : raises
    Lesson ..> LessonContentUpdatedEvent : raises
    Lesson ..> MaterialUploadedEvent : raises

    %% Infrastructure Implementation
    CourseRepository ..|> ICourseRepository : implements
    CourseRepository --> ContentDbContext : uses
    ContentDbContext --> Course : manages
    ContentDbContext --> OutboxMessage : stores

    %% Outbox Mechanism
    ContentOutboxInterceptor --> OutboxMessage : creates
    ContentOutboxInterceptor --> AggregateRoot : reads_events
    ContentOutboxPublisher --> ContentDbContext : reads_pending
    ContentOutboxPublishJob --> ContentOutboxPublisher : triggers

    %% ==========================================
    %% 8. STYLING (EXACT IDENTITY MATCH)
    %% ==========================================
    classDef aggregate fill:#e1f5fe,stroke:#0277bd,stroke-width:2px
    classDef valueObject fill:#f3e5f5,stroke:#7b1fa2,stroke-width:1px
    classDef domainEvent fill:#e8f5e8,stroke:#2e7d32,stroke-width:1px
    classDef repository fill:#fff3e0,stroke:#ef6c00,stroke-width:1px
    classDef infrastructure fill:#fce4ec,stroke:#c2185b,stroke-width:1px
    classDef exception fill:#ffebee,stroke:#c62828,stroke-width:1px

    %% Applying Styles
    class Course aggregate
    class CourseModule,Lesson,Material valueObject
    class CourseStatus,CourseLevel,MaterialType valueObject
    class CourseCreatedEvent,CoursePublishedEvent,ModuleAddedEvent,LessonContentUpdatedEvent,MaterialUploadedEvent domainEvent
    class ICourseRepository,IMaterialRepository,CourseRepository repository
    class ContentDbContext,OutboxMessage,ContentOutboxInterceptor,ContentOutboxPublisher,ContentOutboxPublishJob infrastructure
    class DomainException,CourseDomainException exception
```