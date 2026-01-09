```mermaid
classDiagram
    %% =========================================================
    %% 1. SHARED KERNEL
    %% =========================================================
    class AggregateRoot {
        <<abstract>>
        -List~object~ _domainEvents
        +IReadOnlyCollection~object~ DomainEvents
        #AddDomainEvent(object domainEvent) void
    }

    class Entity {
        <<abstract>>
        +Guid Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
    }

    %% =========================================================
    %% 2. DOMAIN LAYER (Your Code Logic)
    %% =========================================================
    class Course {
        +string Title
        +string Description
        +Guid InstructorId
        +CourseStatus Status
        -List~CourseModule~ _modules
        +IReadOnlyCollection~CourseModule~ Modules
        +AddModule(string title) void
        +Publish() void
        +GetMaterial(Guid materialId) Material?
    }

    class CourseModule {
        +Guid CourseId
        +string Title
        +int OrderIndex
        -List~Lesson~ _lessons
        +IReadOnlyCollection~Lesson~ Lessons
        +AddLesson(string title) void
    }

    class Lesson {
        +Guid ModuleId
        +string Title
        +int OrderIndex
        -List~Material~ _materials
        +IReadOnlyCollection~Material~ Materials
        +AttachMaterial(Material material) void
        +RemoveMaterial(Guid materialId) void
    }

    %% --- THIS MATCHES YOUR PROVIDED CODE SNIPPET ---
    class Material {
        +Guid CourseId
        +Guid? LessonId
        +Guid UploaderId
        +string Title
        +string? Description
        +string FileName
        +string FilePath
        +string ContentType
        +long FileSize
        -Material()
        +Material(Guid courseId, Guid? lessonId, string title, IFormFile file)
        +UpdateMetadata(string title, string? description, Guid? lessonId) void
        +ReplaceFile(string newPath, string newName, long newSize, string newType) void
    }

    %% =========================================================
    %% 3. DOMAIN EVENTS (For Outbox)
    %% =========================================================
    class MaterialUploadedEvent {
        +Guid CourseId
        +Guid MaterialId
        +string FileName
        +long FileSize
        +DateTime OccurredOn
    }

    class MaterialDeletedEvent {
        +Guid CourseId
        +Guid MaterialId
    }

    class MaterialFileReplacedEvent {
        +Guid CourseId
        +Guid MaterialId
        +string NewFileName
        +long NewFileSize
    }

    %% =========================================================
    %% 4. REPOSITORIES & SERVICE INTERFACES
    %% =========================================================
    class IMaterialRepository {
        <<interface>>
        +GetByIdAsync(Guid id) Task~Material?~
        +GetMaterialsAsync(Guid courseId, Guid? lessonId, string search, int page) Task~PagedResult~
        +AddAsync(Material material) Task
        +UpdateAsync(Material material) Task
        +DeleteAsync(Material material) Task
    }

    class MaterialService {
        %% These match your endpoints explicitly
        -IMaterialRepository _repo
        -ContentDbContext _db
        +UploadAsync(Guid courseId, MaterialUploadRequest req, string path, long size, Guid userId) Task~MaterialDto~
        +GetMaterialsAsync(Guid courseId, Guid? lessonId, string search, int page) Task
        +GetByIdAsync(Guid materialId) Task~MaterialDto?~
        +UpdateAsync(Guid materialId, string title, string? desc, Guid? lessonId) Task~MaterialDto?~
        +ReplaceFileAsync(Guid materialId, IFormFile file, string newPath) Task~MaterialDto?~
        +DeleteAsync(Guid materialId) Task~bool~
    }

    %% =========================================================
    %% 5. INFRASTRUCTURE & OUTBOX (The Complex Part)
    %% =========================================================
    class ContentDbContext {
        +DbSet~Course~ Courses
        +DbSet~Material~ Materials
        +DbSet~OutboxMessage~ OutboxMessages
        +SaveChangesAsync() Task~int~
    }

    class OutboxMessage {
        +Guid Id
        +string Type
        +string Payload
        +DateTime OccurredAt
        +DateTime? ProcessedAt
        +string? Error
        +int RetryCount
        +FromEvent(object @event)$ OutboxMessage
    }

    class ContentOutboxInterceptor {
        +SavingChangesAsync(DbContextEventData eventData, InterceptionResult~int~ result)
        -ConvertEventsToOutboxMessages()
    }

    class ContentOutboxPublishJob {
        -ContentDbContext _db
        +Execute(IJobExecutionContext context) Task
    }

    %% =========================================================
    %% 6. RELATIONSHIPS (The Arrows You Requested)
    %% =========================================================
    
    %% Inheritance
    Course --|> AggregateRoot : inherits
    Material --|> Entity : inherits
    
    %% Structure
    Course "1" *-- "many" CourseModule : contains
    CourseModule "1" *-- "many" Lesson : contains
    Lesson "1" *-- "many" Material : contains

    %% Logic Flow
    MaterialService --> IMaterialRepository : uses
    MaterialService --> ContentDbContext : writes_to
    MaterialService ..> Material : creates/updates

    %% Events
    Material ..> MaterialUploadedEvent : raises
    Material ..> MaterialFileReplacedEvent : raises
    Material ..> MaterialDeletedEvent : raises

    %% Outbox Mechanism
    ContentDbContext --> OutboxMessage : stores
    ContentOutboxInterceptor --> OutboxMessage : creates_from_events
    ContentOutboxInterceptor --> AggregateRoot : intercepts
    ContentOutboxPublishJob --> ContentDbContext : polls_messages

    %% =========================================================
    %% 7. STYLING
    %% =========================================================
    classDef aggregate fill:#e1f5fe,stroke:#0277bd,stroke-width:2px
    classDef entity fill:#f3e5f5,stroke:#7b1fa2,stroke-width:1px
    classDef domainEvent fill:#e8f5e8,stroke:#2e7d32,stroke-width:1px
    classDef repository fill:#fff3e0,stroke:#ef6c00,stroke-width:1px
    classDef infrastructure fill:#fce4ec,stroke:#c2185b,stroke-width:1px

    class Course aggregate
    class Material,Lesson,CourseModule entity
    class MaterialUploadedEvent,MaterialDeletedEvent,MaterialFileReplacedEvent domainEvent
    class IMaterialRepository,MaterialService repository
    class OutboxMessage,ContentOutboxInterceptor,ContentOutboxPublishJob,ContentDbContext infrastructure
```