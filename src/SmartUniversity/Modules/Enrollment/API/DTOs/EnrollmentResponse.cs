using SmartUniversity.Modules.Enrollment.Domain.Enums;

namespace SmartUniversity.Modules.Enrollment.Api.DTOs;

public record EnrollmentResponse(
    Guid EnrollmentId,
    Guid CourseId,
    Guid StudentId,
    DateTime EnrollmentDate,
    EnrollmentStatus Status,
    decimal ProgressPercentage
);
