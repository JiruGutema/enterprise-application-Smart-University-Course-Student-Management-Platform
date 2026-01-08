using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Application.Queries;
using SmartUniversity.Modules.GradingAndAssessment.Application.Services;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Services;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Handlers;

public class GetAssignmentsByCourseQueryHandler : IRequestHandler<GetAssignmentsByCourseQuery, List<AssignmentResponse>>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public GetAssignmentsByCourseQueryHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<AssignmentResponse>> Handle(GetAssignmentsByCourseQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByCourseIdAsync(request.CourseId);

        return assignments.Select(a => new AssignmentResponse(
            a.AssignmentId,
            a.CourseId,
            a.Title,
            a.Description,
            a.Type,
            a.DueDate,
            a.MaxScore,
            a.Weight,
            a.CreatedAt
        )).ToList();
    }
}

public class GetStudentGradeSummaryQueryHandler : IRequestHandler<GetStudentGradeSummaryQuery, StudentGradeSummaryResponse>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly GradeCalculationService _gradeCalculationService;
    private readonly IEnrollmentLookupService _enrollmentLookupService;

    public GetStudentGradeSummaryQueryHandler(
        IAssignmentRepository assignmentRepository, 
        IGradeRepository gradeRepository,
        GradeCalculationService gradeCalculationService,
        IEnrollmentLookupService enrollmentLookupService)
    {
        _assignmentRepository = assignmentRepository;
        _gradeRepository = gradeRepository;
        _gradeCalculationService = gradeCalculationService;
        _enrollmentLookupService = enrollmentLookupService;
    }

    public async Task<StudentGradeSummaryResponse> Handle(GetStudentGradeSummaryQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByCourseIdAsync(request.CourseId);
        
        // Get enrollment ID from cache
        var enrollmentId = await _enrollmentLookupService.GetEnrollmentIdAsync(request.StudentId, request.CourseId, cancellationToken);
        if (enrollmentId == null)
            throw new InvalidOperationException($"No active enrollment found for student {request.StudentId} in course {request.CourseId}");

        var grades = await _gradeRepository.GetByEnrollmentIdAsync(enrollmentId.Value);

        // Get course title from cache
        var courseTitle = await _enrollmentLookupService.GetCourseTitleAsync(request.CourseId, cancellationToken) ?? "Unknown Course";

        var breakdown = assignments.Select(a =>
        {
            var grade = grades.FirstOrDefault(g => g.AssignmentId == a.AssignmentId);
            var score = grade?.Score ?? 0;
            var weightedContribution = (score / a.MaxScore) * a.Weight;

            return new GradeBreakdownItem(
                a.Title,
                a.Weight,
                grade?.Score,
                a.MaxScore,
                weightedContribution
            );
        }).ToList();

        var totalWeightedScore = _gradeCalculationService.CalculateWeightedScore(assignments, grades);
        var letterGrade = _gradeCalculationService.GetLetterGrade(totalWeightedScore);
        var completedAssignments = breakdown.Count(b => b.Score.HasValue);

        return new StudentGradeSummaryResponse(
            request.CourseId,
            courseTitle,
            totalWeightedScore,
            letterGrade,
            completedAssignments,
            assignments.Count,
            breakdown,
            DateTime.UtcNow
        );
    }
}