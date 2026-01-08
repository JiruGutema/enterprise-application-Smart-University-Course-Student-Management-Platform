using SmartUniversity.Modules.GradingAndAssessment.Domain.ValueObjects;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;

public record CreateAssignmentRequest(
    string Title,
    string? Description,
    AssignmentType Type,
    DateTime? DueDate,
    decimal MaxScore,
    decimal Weight = 100
);

public record UpdateAssignmentRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    decimal MaxScore,
    decimal Weight
);

public record RecordGradeRequest(
    decimal Score,
    string? Feedback
);

public record BulkGradeRequest(
    Guid StudentId,
    decimal Score,
    string? Feedback
);

public record AssignmentResponse(
    Guid AssignmentId,
    Guid CourseId,
    string Title,
    string? Description,
    AssignmentType Type,
    DateTime? DueDate,
    decimal MaxScore,
    decimal Weight,
    DateTime CreatedAt
);

public record GradeResponse(
    Guid GradeId,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentFullName,
    decimal Score,
    decimal MaxScore,
    decimal Percentage,
    string? Feedback,
    DateTime? GradedAt,
    Guid? GradedByInstructorId
);

public record StudentGradeSummaryResponse(
    Guid CourseId,
    string Title,
    decimal TotalWeightedScore,
    string? CurrentGrade,
    int AssignmentsCompleted,
    int AssignmentsTotal,
    List<GradeBreakdownItem> Breakdown,
    DateTime UpdatedAt
);

public record GradeBreakdownItem(
    string AssignmentTitle,
    decimal Weight,
    decimal? Score,
    decimal MaxScore,
    decimal WeightedContribution
);