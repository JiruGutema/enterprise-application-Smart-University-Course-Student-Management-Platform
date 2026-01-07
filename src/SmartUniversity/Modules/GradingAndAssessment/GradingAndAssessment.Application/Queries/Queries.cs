using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Queries;

public record GetAssignmentsByCourseQuery(Guid CourseId) : IRequest<List<AssignmentResponse>>;

public record GetStudentAssignmentsQuery(Guid CourseId, Guid StudentId) : IRequest<List<AssignmentResponse>>;

public record GetStudentGradeSummaryQuery(Guid CourseId, Guid StudentId) : IRequest<StudentGradeSummaryResponse>;

public record GetGradebookQuery(Guid CourseId) : IRequest<object>;