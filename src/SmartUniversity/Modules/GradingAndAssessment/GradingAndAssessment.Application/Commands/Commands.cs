using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Domain.ValueObjects;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Commands;

public record CreateAssignmentCommand(
    Guid CourseId,
    string Title,
    string? Description,
    AssignmentType Type,
    DateTime? DueDate,
    decimal MaxScore,
    decimal Weight
) : IRequest<AssignmentResponse>;

public record UpdateAssignmentCommand(
    Guid AssignmentId,
    string Title,
    string? Description,
    DateTime? DueDate,
    decimal MaxScore,
    decimal Weight
) : IRequest<AssignmentResponse>;

public record DeleteAssignmentCommand(Guid AssignmentId) : IRequest;

public record RecordGradeCommand(
    Guid AssignmentId,
    Guid StudentId,
    decimal Score,
    string? Feedback,
    Guid? GradedByInstructorId
) : IRequest<GradeResponse>;

public record BulkRecordGradesCommand(
    Guid AssignmentId,
    List<BulkGradeRequest> Grades,
    Guid? GradedByInstructorId
) : IRequest<List<GradeResponse>>;