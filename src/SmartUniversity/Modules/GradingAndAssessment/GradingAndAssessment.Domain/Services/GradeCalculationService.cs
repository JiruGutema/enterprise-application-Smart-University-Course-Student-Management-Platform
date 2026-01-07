using SmartUniversity.Modules.GradingAndAssessment.Domain.Entities;

namespace SmartUniversity.Modules.GradingAndAssessment.Domain.Services;

public class GradeCalculationService
{
    public decimal CalculateWeightedScore(List<Assignment> assignments, List<Grade> grades)
    {
        if (!assignments.Any()) return 0;

        var totalWeight = assignments.Sum(a => a.Weight);
        if (totalWeight == 0) return 0;

        var weightedSum = assignments.Sum(assignment =>
        {
            var grade = grades.FirstOrDefault(g => g.AssignmentId == assignment.AssignmentId);
            if (grade == null) return 0;

            var percentage = grade.Score / assignment.MaxScore * 100;
            return percentage * (assignment.Weight / totalWeight);
        });

        return weightedSum;
    }

    public string GetLetterGrade(decimal percentage)
    {
        return percentage switch
        {
            >= 90 => "A",
            >= 80 => "B", 
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }

    public bool IsPassingGrade(decimal percentage)
    {
        return percentage >= 60;
    }
}