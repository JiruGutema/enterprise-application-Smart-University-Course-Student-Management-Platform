namespace SmartUniversity.Modules.GradingAndAssessment.Domain.ValueObjects;

public record Score
{
    public decimal Value { get; }
    public decimal MaxValue { get; }
    public decimal Percentage => MaxValue > 0 ? (Value / MaxValue) * 100 : 0;

    public Score(decimal value, decimal maxValue)
    {
        if (value < 0) throw new ArgumentException("Score cannot be negative");
        if (maxValue <= 0) throw new ArgumentException("Max score must be positive");
        if (value > maxValue) throw new ArgumentException("Score cannot exceed max score");

        Value = value;
        MaxValue = maxValue;
    }
}

public record Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value < 0 || value > 100) 
            throw new ArgumentException("Weight must be between 0 and 100");
        Value = value;
    }
}

public record AssignmentTitle
{
    public string Value { get; }

    public AssignmentTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Assignment title cannot be empty");
        if (value.Length > 200)
            throw new ArgumentException("Assignment title cannot exceed 200 characters");

        Value = value.Trim();
    }
}