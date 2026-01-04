namespace SmartUniversity.Modules.Enrollment.Domain.Entities;

public class Attendance
{
    public Guid Id { get; private set; }
    public DateOnly Date { get; private set; }
    public bool IsPresent { get; private set; }
    public string? Notes { get; private set; }

    private Attendance() { }

    public Attendance(DateOnly date, bool isPresent, string? notes = null)
    {
        Id = Guid.NewGuid();
        Date = date;
        IsPresent = isPresent;
        Notes = notes;
    }
}
