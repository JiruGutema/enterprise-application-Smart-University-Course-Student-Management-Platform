using MediatR;
using SmartUniversity.Modules.GradingAndAssessment.Application.Commands;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;

namespace SmartUniversity.Modules.GradingAndAssessment.Application.Handlers;

public class RecordGradeCommandHandler : IRequestHandler<RecordGradeCommand, GradeResponse>
{
    private readonly IGradeRepository _gradeRepository;

    public RecordGradeCommandHandler(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<GradeResponse> Handle(RecordGradeCommand request, CancellationToken cancellationToken)
    {
        // Note: In real implementation, you'd need to get enrollmentId from StudentId + CourseId
        var enrollmentId = Guid.NewGuid(); // Placeholder

        var existingGrade = await _gradeRepository.GetByEnrollmentAndAssignmentAsync(enrollmentId, request.AssignmentId);
        
        if (existingGrade != null)
        {
            existingGrade.UpdateScore(request.Score, request.Feedback, request.GradedByInstructorId);
            await _gradeRepository.UpdateAsync(existingGrade);
            
            return new GradeResponse(
                existingGrade.GradeId,
                existingGrade.EnrollmentId,
                request.StudentId,
                "Student Name", // Placeholder
                existingGrade.Score,
                100, // Placeholder max score
                existingGrade.Score,
                existingGrade.Feedback,
                existingGrade.GradedAt,
                existingGrade.GradedByInstructorId
            );
        }

        var grade = new Grade(enrollmentId, request.AssignmentId, request.Score, request.Feedback, request.GradedByInstructorId);
        await _gradeRepository.AddAsync(grade);

        return new GradeResponse(
            grade.GradeId,
            grade.EnrollmentId,
            request.StudentId,
            "Student Name", // Placeholder
            grade.Score,
            100, // Placeholder max score
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