# Grading and Assessment Module Class Diagram

This diagram shows the architecture of the GradingAndAssessment module following Clean Architecture and CQRS patterns with cross-module caching.

```mermaid
classDiagram
    %% =========================================================
    %% API LAYER
    %% =========================================================
    class AssessmentsController {
        -IMediator _mediators
        +CreateAssignment(Guid courseId, CreateAssignmentRequest request) Task~IActionResult~
        +GetAssignments(Guid courseId) Task~IActionResult~
        +UpdateAssignment(Guid assignmentId, UpdateAssignmentRequest request) Task~IActionResult~
        +DeleteAssignment(Guid assignmentId) Task~IActionResult~
        +RecordGrade(Guid assignmentId, Guid studentId, RecordGradeRequest request) Task~IActionResult~
        +BulkRecordGrades(Guid assignmentId, List~BulkGradeRequest~ request) Task~IActionResult~
        +GetMyAssignments(Guid courseId) Task~IActionResult~
        +GetMyGradeSummary(Guid courseId) Task~IActionResult~
        +GetGradebook(Guid courseId) Task~IActionResult~
    }

    %% =========================================================
    %% APPLICATION LAYER - DTOs
    %% =========================================================
    class CreateAssignmentRequest {
        +string Title
        +string? Description
        +AssignmentType Type
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
    }

    class UpdateAssignmentRequest {
        +string Title
        +string? Description
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
    }

    class RecordGradeRequest {
        +decimal Score
        +string? Feedback
    }

    class BulkGradeRequest {
        +Guid StudentId
        +decimal Score
        +string? Feedback
    }

    class AssignmentDto {
        +Guid AssignmentId
        +Guid CourseId
        +string Title
        +string? Description
        +AssignmentType Type
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
        +DateTime CreatedAt
    }

    class GradeDto {
        +Guid GradeId
        +Guid EnrollmentId
        +Guid AssignmentId
        +decimal Score
        +string? Feedback
        +Guid? GradedByInstructorId
        +DateTime? GradedAt
    }

    class StudentGradeSummaryDto {
        +Guid StudentId
        +Guid CourseId
        +decimal OverallGrade
        +List~GradeDto~ Grades
        +List~AssignmentDto~ PendingAssignments
    }

    class GradebookDto {
        +Guid CourseId
        +List~AssignmentDto~ Assignments
        +List~StudentGradeRowDto~ StudentGrades
    }

    class StudentGradeRowDto {
        +Guid StudentId
        +string StudentName
        +List~decimal?~ Scores
        +decimal? OverallGrade
    }

    %% =========================================================
    %% APPLICATION LAYER - COMMANDS
    %% =========================================================
    class CreateAssignmentCommand {
        +Guid CourseId
        +string Title
        +string? Description
        +AssignmentType Type
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
        +CreateAssignmentCommand(Guid courseId, string title, ...)
    }

    class UpdateAssignmentCommand {
        +Guid AssignmentId
        +string Title
        +string? Description
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
        +UpdateAssignmentCommand(Guid assignmentId, string title, ...)
    }

    class DeleteAssignmentCommand {
        +Guid AssignmentId
        +DeleteAssignmentCommand(Guid assignmentId)
    }

    class RecordGradeCommand {
        +Guid AssignmentId
        +Guid StudentId
        +decimal Score
        +string? Feedback
        +Guid GradedByInstructorId
        +RecordGradeCommand(Guid assignmentId, Guid studentId, ...)
    }

    class BulkRecordGradesCommand {
        +Guid AssignmentId
        +List~BulkGradeRequest~ Grades
        +Guid GradedByInstructorId
        +BulkRecordGradesCommand(Guid assignmentId, List~BulkGradeRequest~ grades, Guid instructorId)
    }

    %% =========================================================
    %% APPLICATION LAYER - QUERIES
    %% =========================================================
    class GetAssignmentsByCourseQuery {
        +Guid CourseId
        +GetAssignmentsByCourseQuery(Guid courseId)
    }

    class GetStudentAssignmentsQuery {
        +Guid CourseId
        +Guid StudentId
        +GetStudentAssignmentsQuery(Guid courseId, Guid studentId)
    }

    class GetStudentGradeSummaryQuery {
        +Guid CourseId
        +Guid StudentId
        +GetStudentGradeSummaryQuery(Guid courseId, Guid studentId)
    }

    class GetGradebookQuery {
        +Guid CourseId
        +GetGradebookQuery(Guid courseId)
    }

    %% =========================================================
    %% APPLICATION LAYER - HANDLERS
    %% =========================================================
    class CreateAssignmentHandler {
        -IAssignmentRepository _assignmentRepository
        +Handle(CreateAssignmentCommand request, CancellationToken ct) Task~AssignmentDto~
    }

    class UpdateAssignmentHandler {
        -IAssignmentRepository _assignmentRepository
        +Handle(UpdateAssignmentCommand request, CancellationToken ct) Task~AssignmentDto~
    }

    class DeleteAssignmentHandler {
        -IAssignmentRepository _assignmentRepository
        +Handle(DeleteAssignmentCommand request, CancellationToken ct) Task
    }

    class RecordGradeHandler {
        -IGradeRepository _gradeRepository
        -IEnrollmentLookupService _enrollmentService
        +Handle(RecordGradeCommand request, CancellationToken ct) Task~GradeDto~
    }

    class BulkRecordGradesHandler {
        -IGradeRepository _gradeRepository
        -IEnrollmentLookupService _enrollmentService
        +Handle(BulkRecordGradesCommand request, CancellationToken ct) Task~List~GradeDto~~
    }

    class GetAssignmentsByCourseHandler {
        -IAssignmentRepository _assignmentRepository
        +Handle(GetAssignmentsByCourseQuery request, CancellationToken ct) Task~List~AssignmentDto~~
    }

    class GetStudentGradeSummaryHandler {
        -IGradeRepository _gradeRepository
        -IAssignmentRepository _assignmentRepository
        -GradeCalculationService _gradeCalculationService
        +Handle(GetStudentGradeSummaryQuery request, CancellationToken ct) Task~StudentGradeSummaryDto~
    }

    class GetGradebookHandler {
        -IAssignmentRepository _assignmentRepository
        -IGradeRepository _gradeRepository
        -IEnrollmentLookupService _enrollmentService
        +Handle(GetGradebookQuery request, CancellationToken ct) Task~GradebookDto~
    }

    %% =========================================================
    %% APPLICATION LAYER - SERVICES
    %% =========================================================
    class IEnrollmentLookupService {
        <<interface>>
        +GetEnrollmentIdAsync(Guid studentId, Guid courseId) Task~Guid?~
        +GetStudentsByCourseAsync(Guid courseId) Task~List~StudentCache~~
    }

    class EnrollmentLookupService {
        -GradingDbContext _context
        +GetEnrollmentIdAsync(Guid studentId, Guid courseId) Task~Guid?~
        +GetStudentsByCourseAsync(Guid courseId) Task~List~StudentCache~~
    }

    %% =========================================================
    %% DOMAIN LAYER - AGGREGATES
    %% =========================================================
    class Assignment {
        +Guid AssignmentId
        +Guid CourseId
        +string Title
        +string? Description
        +AssignmentType Type
        +DateTime? DueDate
        +decimal MaxScore
        +decimal Weight
        +DateTime CreatedAt
        +DateTime UpdatedAt
        -Assignment()
        +Assignment(Guid courseId, string title, string? description, AssignmentType type, DateTime? dueDate, decimal maxScore, decimal weight)
        +Update(string title, string? description, DateTime? dueDate, decimal maxScore, decimal weight) void
    }

    class Grade {
        +Guid GradeId
        +Guid EnrollmentId
        +Guid AssignmentId
        +decimal Score
        +string? Feedback
        +Guid? GradedByInstructorId
        +DateTime? GradedAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
        -Grade()
        +Grade(Guid enrollmentId, Guid assignmentId, decimal score, string? feedback, Guid? gradedByInstructorId)
        +UpdateScore(decimal score, string? feedback, Guid? gradedByInstructorId) void
    }

    %% =========================================================
    %% DOMAIN LAYER - VALUE OBJECTS
    %% =========================================================
    class AssignmentType {
        <<enumeration>>
        Quiz
        Exam
        Assignment
        Project
        Participation
    }

    class GradeScale {
        +decimal MinScore
        +decimal MaxScore
        +string LetterGrade
        +GradeScale(decimal minScore, decimal maxScore, string letterGrade)
        +CalculateLetterGrade(decimal score)$ string
    }

    %% =========================================================
    %% DOMAIN LAYER - EVENTS
    %% =========================================================
    class AssignmentCreatedEvent {
        +Guid AssignmentId
        +Guid CourseId
        +string Title
        +DateTime? DueDate
        +AssignmentCreatedEvent(Guid assignmentId, Guid courseId, string title, DateTime? dueDate)
    }

    class AssignmentUpdatedEvent {
        +Guid AssignmentId
        +string Title
        +DateTime? DueDate
        +AssignmentUpdatedEvent(Guid assignmentId, string title, DateTime? dueDate)
    }

    class GradeRecordedEvent {
        +Guid GradeId
        +Guid EnrollmentId
        +Guid AssignmentId
        +decimal Score
        +GradeRecordedEvent(Guid gradeId, Guid enrollmentId, Guid assignmentId, decimal score)
    }

    class GradeUpdatedEvent {
        +Guid GradeId
        +decimal Score
        +string? Feedback
        +GradeUpdatedEvent(Guid gradeId, decimal score, string? feedback)
    }

    %% =========================================================
    %% DOMAIN LAYER - REPOSITORIES
    %% =========================================================
    class IAssignmentRepository {
        <<interface>>
        +CreateAsync(Assignment assignment) Task~Assignment~
        +GetByIdAsync(Guid assignmentId) Task~Assignment?~
        +GetByCourseIdAsync(Guid courseId) Task~List~Assignment~~
        +UpdateAsync(Assignment assignment) Task
        +DeleteAsync(Guid assignmentId) Task
    }

    class IGradeRepository {
        <<interface>>
        +CreateAsync(Grade grade) Task~Grade~
        +GetByIdAsync(Guid gradeId) Task~Grade?~
        +GetByAssignmentAndStudentAsync(Guid assignmentId, Guid enrollmentId) Task~Grade?~
        +GetByEnrollmentIdAsync(Guid enrollmentId) Task~List~Grade~~
        +GetByCourseIdAsync(Guid courseId) Task~List~Grade~~
        +UpdateAsync(Grade grade) Task
        +DeleteAsync(Guid gradeId) Task
    }

    %% =========================================================
    %% DOMAIN LAYER - SERVICES
    %% =========================================================
    class GradeCalculationService {
        +CalculateOverallGrade(List~Grade~ grades, List~Assignment~ assignments) decimal
        +CalculateWeightedAverage(List~Grade~ grades, List~Assignment~ assignments) decimal
        +GetLetterGrade(decimal numericGrade) string
    }

    %% =========================================================
    %% INFRASTRUCTURE LAYER - PERSISTENCE
    %% =========================================================
    class GradingDbContext {
        +DbSet~Assignment~ Assignments
        +DbSet~Grade~ Grades
        +DbSet~OutboxMessage~ OutboxMessages
        +DbSet~EnrollmentCache~ EnrollmentCache
        +DbSet~CourseCache~ CourseCache
        +DbSet~StudentCache~ StudentCache
        +GradingDbContext(DbContextOptions~GradingDbContext~ options)
        #OnModelCreating(ModelBuilder modelBuilder) void
    }

    class AssignmentRepository {
        -GradingDbContext _context
        +CreateAsync(Assignment assignment) Task~Assignment~
        +GetByIdAsync(Guid assignmentId) Task~Assignment?~
        +GetByCourseIdAsync(Guid courseId) Task~List~Assignment~~
        +UpdateAsync(Assignment assignment) Task
        +DeleteAsync(Guid assignmentId) Task
    }

    class GradeRepository {
        -GradingDbContext _context
        +CreateAsync(Grade grade) Task~Grade~
        +GetByIdAsync(Guid gradeId) Task~Grade?~
        +GetByAssignmentAndStudentAsync(Guid assignmentId, Guid enrollmentId) Task~Grade?~
        +GetByEnrollmentIdAsync(Guid enrollmentId) Task~List~Grade~~
        +GetByCourseIdAsync(Guid courseId) Task~List~Grade~~
        +UpdateAsync(Grade grade) Task
        +DeleteAsync(Guid gradeId) Task
    }

    %% =========================================================
    %% INFRASTRUCTURE LAYER - CACHE
    %% =========================================================
    class EnrollmentCache {
        +Guid EnrollmentId
        +Guid StudentId
        +Guid CourseId
        +string Status
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class CourseCache {
        +Guid CourseId
        +string Title
        +string Code
        +Guid InstructorId
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class StudentCache {
        +Guid StudentId
        +string FullName
        +string Email
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    %% =========================================================
    %% INFRASTRUCTURE LAYER - OUTBOX
    %% =========================================================
    class OutboxMessage {
        +Guid Id
        +string Type
        +string Data
        +DateTime OccurredOn
        +DateTime? ProcessedOn
        +string? Error
    }

    class GradingOutboxPublisher {
        -GradingDbContext _context
        -IEventBus _eventBus
        +PublishPendingAsync(CancellationToken ct) Task
    }

    class GradingOutboxPublishJob {
        -GradingOutboxPublisher _publisher
        +Execute(IJobExecutionContext context) Task
    }

    %% =========================================================
    %% RELATIONSHIPS
    %% =========================================================
    
    %% API Dependencies
    AssessmentsController --> CreateAssignmentCommand : creates
    AssessmentsController --> UpdateAssignmentCommand : creates
    AssessmentsController --> DeleteAssignmentCommand : creates
    AssessmentsController --> RecordGradeCommand : creates
    AssessmentsController --> BulkRecordGradesCommand : creates
    AssessmentsController --> GetAssignmentsByCourseQuery : creates
    AssessmentsController --> GetStudentAssignmentsQuery : creates
    AssessmentsController --> GetStudentGradeSummaryQuery : creates
    AssessmentsController --> GetGradebookQuery : creates

    %% Command/Query Handler relationships
    CreateAssignmentCommand --> CreateAssignmentHandler : handled by
    UpdateAssignmentCommand --> UpdateAssignmentHandler : handled by
    DeleteAssignmentCommand --> DeleteAssignmentHandler : handled by
    RecordGradeCommand --> RecordGradeHandler : handled by
    BulkRecordGradesCommand --> BulkRecordGradesHandler : handled by
    GetAssignmentsByCourseQuery --> GetAssignmentsByCourseHandler : handled by
    GetStudentGradeSummaryQuery --> GetStudentGradeSummaryHandler : handled by
    GetGradebookQuery --> GetGradebookHandler : handled by

    %% Handler Dependencies
    CreateAssignmentHandler --> IAssignmentRepository : uses
    UpdateAssignmentHandler --> IAssignmentRepository : uses
    DeleteAssignmentHandler --> IAssignmentRepository : uses
    RecordGradeHandler --> IGradeRepository : uses
    RecordGradeHandler --> IEnrollmentLookupService : uses
    BulkRecordGradesHandler --> IGradeRepository : uses
    BulkRecordGradesHandler --> IEnrollmentLookupService : uses
    GetAssignmentsByCourseHandler --> IAssignmentRepository : uses
    GetStudentGradeSummaryHandler --> IGradeRepository : uses
    GetStudentGradeSummaryHandler --> IAssignmentRepository : uses
    GetStudentGradeSummaryHandler --> GradeCalculationService : uses
    GetGradebookHandler --> IAssignmentRepository : uses
    GetGradebookHandler --> IGradeRepository : uses
    GetGradebookHandler --> IEnrollmentLookupService : uses

    %% Service Implementation
    EnrollmentLookupService ..|> IEnrollmentLookupService : implements
    EnrollmentLookupService --> GradingDbContext : uses

    %% Domain Events
    Assignment --> AssignmentCreatedEvent : raises
    Assignment --> AssignmentUpdatedEvent : raises
    Grade --> GradeRecordedEvent : raises
    Grade --> GradeUpdatedEvent : raises

    %% Repository Implementation
    AssignmentRepository ..|> IAssignmentRepository : implements
    AssignmentRepository --> GradingDbContext : uses
    GradeRepository ..|> IGradeRepository : implements
    GradeRepository --> GradingDbContext : uses

    %% Domain Model
    Assignment --> AssignmentType : uses
    Grade --> GradeScale : uses

    %% Infrastructure Dependencies
    GradingDbContext --> Assignment : maps
    GradingDbContext --> Grade : maps
    GradingDbContext --> OutboxMessage : maps
    GradingDbContext --> EnrollmentCache : maps
    GradingDbContext --> CourseCache : maps
    GradingDbContext --> StudentCache : maps

    %% Outbox Pattern
    GradingOutboxPublisher --> GradingDbContext : reads from
    GradingOutboxPublisher --> OutboxMessage : processes
    GradingOutboxPublishJob --> GradingOutboxPublisher : uses

    %% =========================================================
    %% STYLING
    %% =========================================================
   
```