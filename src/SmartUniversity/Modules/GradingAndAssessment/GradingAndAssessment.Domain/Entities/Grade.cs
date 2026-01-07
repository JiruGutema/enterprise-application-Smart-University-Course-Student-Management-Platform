using SmartUniversity.Modules.GradingAndAssessment.Domain.Events;
using SmartUniversity.Shared.Kernel;

namespace SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;

public class Grade : AggregateRoot
{
    public Guid GradeId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid AssignmentId { get; private set; }
    public decimal Score { get; private set; }
    public string? Feedback { get; private set; }
    public Guid? GradedByInstructorId { get; private set; }
    public DateTime? GradedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Grade() { } // EF Core

    public Grade(Guid enrollmentId, Guid assignmentId, decimal score, string? feedback, Guid? gradedByInstructorId)
    {
        GradeId = Guid.NewGuid();
        EnrollmentId = enrollmentId;
        AssignmentId = assignmentId;
        Score = score;
        Feedback = feedback;
        GradedByInstructorId = gradedByInstructorId;
        GradedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new GradeRecordedEvent(GradeId, EnrollmentId, AssignmentId, Score));
    }

    public void UpdateScore(decimal score, string? feedback, Guid? gradedByInstructorId)
    {
        Score = score;
        Feedback = feedback;
        GradedByInstructorId = gradedByInstructorId;
        GradedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new GradeUpdatedEvent(GradeId, Score, Feedback));
    }
}