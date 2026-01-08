namespace SmartUniversity.Modules.Courses.Domain.ValueObjects;


 
public sealed record CourseCode
{
    public string Value { get; }

    private CourseCode(string value)
    {
        Value = value;
    }

    public static CourseCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Course code cannot be empty.");

        value = value.Trim().ToUpperInvariant();

        // Simple validation rule (you can improve later)
        if (value.Length < 4 || value.Length > 10)
            throw new ArgumentException("Invalid course code format.");

        return new CourseCode(value);
    }

    public override string ToString() => Value;
}
