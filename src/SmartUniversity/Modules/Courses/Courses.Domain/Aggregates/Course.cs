using SmartUniversity.Modules.Courses.Domain.ValueObjects;
using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Courses.Domain.Common;
using SmartUniversity.Modules.Courses.Domain.Enums;

namespace SmartUniversity.Modules.Courses.Domain.Aggregates;

public sealed class Course : AggregateRoot
{
    private readonly List<CourseCode> _prerequisites = new();

    public Guid Id { get; private set; }
    public CourseCode Code { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public CourseStatus Status { get; private set; }
    public Guid InstructorId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public IReadOnlyCollection<CourseCode> Prerequisites => _prerequisites.AsReadOnly();

    // ADD THESE PROPERTIES
    public bool IsPublished => Status == CourseStatus.Published;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Course() { } // For EF Core

    private Course(Guid id, CourseCode code, string title, string description, DateTime startDate, DateTime endDate, Guid instructorId)
    {
        Id = id;
        Code = code;
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        InstructorId = instructorId;
        Status = CourseStatus.Draft;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new CourseCreatedEvent(Id, Title, Code.Value, InstructorId));
    }

    public static Course Create(string title, CourseCode code, string description, DateTime startDate, DateTime endDate, Guid instructorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title is required.");

        return new Course(Guid.NewGuid(), code, title.Trim(), description, startDate, endDate, instructorId);
    }

    public void Update(
        string? title,
        string? code,
        string? description,
        DateTime? startDate,
        DateTime? endDate,
        Guid? instructorId,
        List<CourseCode>? prerequisites
    )
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title.Trim();

        if (!string.IsNullOrWhiteSpace(code))
            Code = CourseCode.Create(code);

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (startDate.HasValue)
            StartDate = startDate.Value;

        if (endDate.HasValue)
            EndDate = endDate.Value;

        if (instructorId.HasValue && instructorId.Value != Guid.Empty)
            ChangeInstructor(instructorId.Value);

        if (prerequisites != null && prerequisites.Any())
            SetPrerequisites(prerequisites);

        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == CourseStatus.Published)
            throw new InvalidOperationException("Course is already published.");

        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CoursePublishedEvent(Id));
    }

    public void Unpublish()
    {
        if (Status == CourseStatus.Draft)
            throw new InvalidOperationException("Course is already unpublished.");

        Status = CourseStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CourseUnpublishedEvent(Id));
    }

    public void SetPrerequisites(IEnumerable<CourseCode> prerequisites)
    {
        _prerequisites.Clear();
        _prerequisites.AddRange(prerequisites);
    }

    public void AddPrerequisite(CourseCode prerequisite)
    {
        if (_prerequisites.Contains(prerequisite))
            return;

        _prerequisites.Add(prerequisite);
    }

    public void ChangeInstructor(Guid newInstructorId)
    {
        if (newInstructorId == Guid.Empty)
            throw new ArgumentException("Instructor ID is invalid.");

        InstructorId = newInstructorId;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CourseInstructorChangedEvent(Id, newInstructorId));
    }
}
