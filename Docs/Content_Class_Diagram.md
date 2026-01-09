```mermaid
classDiagram
    %% =========================================================
    %% 1. SHARED KERNEL (Base Classes)
    %% =========================================================
    class Entity {
        <<abstract>>
        +Guid Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
    }

    class AggregateRoot {
        <<abstract>>
        -List~object~ _domainEvents
        +IReadOnlyCollection~object~ DomainEvents
        #AddDomainEvent(object domainEvent) void
    }

    %% =========================================================
    %% 2. DOMAIN LAYER (Strictly Your Code)
    %% =========================================================
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
        +Material(Guid courseId, string title, string fileName, string filePath)
        +UpdateMetadata(string title, string? description, Guid? lessonId) void
        +ReplaceFile(string fileName, string filePath, long fileSize, string contentType) void
    }

    %% These exist as context (CourseId/LessonId) in your Material entity
    class Course {
        +string Title
        +Guid InstructorId
        -List~Material~ _materials
    }

    class Lesson {
        +Guid ModuleId
        +string Title
    }

    %% =========================================================
    %% 3. APPLICATION LAYER (Services & DTOs from your code)
    %% =========================================================
    class MaterialService {
        -ContentDbContext _context
        +UploadAsync(Guid courseId, MaterialUploadRequest req, string fullPath, long size, Guid userId) Task~MaterialDto~
        +GetMaterialsAsync(Guid courseId, Guid? lessonId, string search, int page, int pageSize) Task~object~
        +GetByIdAsync(Guid materialId) Task~MaterialDto?~
        +UpdateAsync(Guid materialId, string title, string? desc, Guid? lessonId) Task~MaterialDto?~
        +ReplaceFileAsync(Guid materialId, IFormFile file, string newPath) Task~MaterialDto?~
        +DeleteAsync(Guid materialId) Task~bool~
        -MapToDto(Material m) MaterialDto
    }

    class MaterialUploadRequest {
        +string Title
        +string? Description
        +Guid? LessonId
        +IFormFile File
    }

    class UpdateMaterialRequest {
        +string Title
        +string? Description
        +Guid? LessonId
    }

    class MaterialDto {
        +Guid Id
        +Guid CourseId
        +Guid? LessonId
        +string Title
        +string FileName
        +string FilePath
        +long FileSize
    }

    %% =========================================================
    %% 4. DOMAIN EVENTS (Required for Outbox)
    %% =========================================================
    class MaterialUploadedEvent {
        +Guid MaterialId
        +Guid CourseId
        +DateTime OccurredOn
    }

    class MaterialDeletedEvent {
        +Guid MaterialId
        +Guid CourseId
    }

    %% =========================================================
    %% 5. INFRASTRUCTURE & PERSISTENCE (From your setup)
    %% =========================================================
    class ContentDbContext {
        +DbSet~Material~ Materials
        +DbSet~Course~ Courses
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
    }

    class ContentOutboxPublishJob {
        +Execute(IJobExecutionContext context) Task
    }

    %% =========================================================
    %% 6. RELATIONSHIPS (Explicit Connections)
    %% =========================================================
    
    %% Inheritance
    Material --|> Entity : inherits
    Material --|> AggregateRoot : inherits
    Course --|> AggregateRoot : inherits

    %% Composition / Containment
    Course "1" *-- "many" Material : contains (via CourseId)
    
    %% Service Dependencies
    MaterialService --> ContentDbContext : uses
    MaterialService ..> Material : creates_and_updates
    MaterialService ..> MaterialDto : returns
    MaterialService ..> MaterialUploadRequest : accepts
    MaterialService ..> UpdateMaterialRequest : accepts

    %% Event Logic
    Material ..> MaterialUploadedEvent : raises
    Material ..> MaterialDeletedEvent : raises

    %% Outbox Flow
    ContentDbContext --> Material : manages
    ContentDbContext --> OutboxMessage : stores
    ContentOutboxInterceptor --> OutboxMessage : creates
    ContentOutboxInterceptor --> AggregateRoot : intercepts_events
    ContentOutboxPublishJob --> ContentDbContext : polls_messages

    %% =========================================================
    %% 7. STYLING (Matching Identity Example)
    %% =========================================================
    classDef aggregate fill:#e1f5fe,stroke:#0277bd,stroke-width:2px
    classDef valueObject fill:#f3e5f5,stroke:#7b1fa2,stroke-width:1px
    classDef domainEvent fill:#e8f5e8,stroke:#2e7d32,stroke-width:1px
    classDef repository fill:#fff3e0,stroke:#ef6c00,stroke-width:1px
    classDef infrastructure fill:#fce4ec,stroke:#c2185b,stroke-width:1px

    %% Applying Styles
    class Material,Course,Lesson aggregate
    class MaterialDto,MaterialUploadRequest,UpdateMaterialRequest valueObject
    class MaterialUploadedEvent,MaterialDeletedEvent domainEvent
    class MaterialService repository
    class ContentDbContext,OutboxMessage,ContentOutboxInterceptor,ContentOutboxPublishJob infrastructure
```