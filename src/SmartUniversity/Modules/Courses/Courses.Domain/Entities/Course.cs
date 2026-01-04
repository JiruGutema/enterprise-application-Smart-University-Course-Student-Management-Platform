namespace Courses.Domain.Entities;
public class Course
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public Guid InstructorId { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsPublished { get; private set; }

    private Course() { } // EF Core

    public Course(
        string title,
        Guid instructorId,
        string? code = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        Id = Guid.NewGuid();
        Title = title;
        InstructorId = instructorId;
        Code = code;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsPublished = false;
    }

    public void Publish()
    {
        IsPublished = true;
    }
}
