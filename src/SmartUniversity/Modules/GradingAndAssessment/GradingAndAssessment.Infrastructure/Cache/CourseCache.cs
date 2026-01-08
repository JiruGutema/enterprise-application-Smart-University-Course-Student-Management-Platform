namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Cache;

public class CourseCache
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}