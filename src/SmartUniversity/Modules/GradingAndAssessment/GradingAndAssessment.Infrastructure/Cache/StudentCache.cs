namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Cache;

public class StudentCache
{
    public Guid StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}