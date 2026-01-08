using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.Commands;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Application.Services;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Handlers;

public class RecordGradeCommandHandler : IRequestHandler<RecordGradeCommand, GradeResponse>
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEnrollmentLookupService _enrollmentLookupService;

    public RecordGradeCommandHandler(
        IGradeRepository gradeRepository,
        IAssignmentRepository assignmentRepository,
        IEnrollmentLookupService enrollmentLookupService)
    {
        _gradeRepository = gradeRepository;
        _assignmentRepository = assignmentRepository;
        _enrollmentLookupService = enrollmentLookupService;
    }

    public async Task<GradeResponse> Handle(RecordGradeCommand request, CancellationToken cancellationToken)
    {
        // Get assignment to retrieve course information
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
            throw new InvalidOperationException($"Assignment {request.AssignmentId} not found");

        // Get enrollment ID from cache
        var enrollmentId = await _enrollmentLookupService.GetEnrollmentIdAsync(request.StudentId, assignment.CourseId, cancellationToken);
        if (enrollmentId == null)
            throw new InvalidOperationException($"No active enrollment found for student {request.StudentId} in course {assignment.CourseId}");

        // Get student name from cache
        var studentName = await _enrollmentLookupService.GetStudentNameAsync(request.StudentId, cancellationToken) ?? "Unknown Student";

        var existingGrade = await _gradeRepository.GetByEnrollmentAndAssignmentAsync(enrollmentId.Value, request.AssignmentId);
        
        if (existingGrade != null)
        {
            existingGrade.UpdateScore(request.Score, request.Feedback, request.GradedByInstructorId);
            await _gradeRepository.UpdateAsync(existingGrade);
            
            return new GradeResponse(
                existingGrade.GradeId,
                existingGrade.EnrollmentId,
                request.StudentId,
                studentName,
                existingGrade.Score,
                assignment.MaxScore,
                existingGrade.Score,
                existingGrade.Feedback,
                existingGrade.GradedAt,
                existingGrade.GradedByInstructorId
            );
        }

        var grade = new Grade(enrollmentId.Value, request.AssignmentId, request.Score, request.Feedback, request.GradedByInstructorId);
        await _gradeRepository.AddAsync(grade);

        return new GradeResponse(
            grade.GradeId,
            grade.EnrollmentId,
            request.StudentId,
            studentName,
            grade.Score,
            assignment.MaxScore,
            grade.Score,
            grade.Feedback,
            grade.GradedAt,
            grade.GradedByInstructorId
        );
    }
}

public class BulkRecordGradesCommandHandler : IRequestHandler<BulkRecordGradesCommand, List<GradeResponse>>
{
    private readonly IMediator _mediator;

    public BulkRecordGradesCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<GradeResponse>> Handle(BulkRecordGradesCommand request, CancellationToken cancellationToken)
    {
        var results = new List<GradeResponse>();

        foreach (var gradeRequest in request.Grades)
        {
            var command = new RecordGradeCommand(
                request.AssignmentId,
                gradeRequest.StudentId,
                gradeRequest.Score,
                gradeRequest.Feedback,
                request.GradedByInstructorId
            );

            var result = await _mediator.Send(command, cancellationToken);
            results.Add(result);
        }

        return results;
    }
}