using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.ValueObjects;

namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class CourseResponse
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid? InstructorId { get; set; }
    public string? InstructorFullName { get; set; }
    public string? InstructorEmail { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPublished { get; set; }
    public string[]? Prerequisites { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ModuleResponse>? Modules { get; set; }

    // Mapper: converts domain Course → DTO
    public static CourseResponse FromDomain(Course course)
    {
        return new CourseResponse
        {
            CourseId = course.Id,
            Title = course.Title,
            Code = course.Code.Value,
            Description = course.Description,
            InstructorId = course.InstructorId,
            // You can fetch instructor full name/email from UserContext later if needed
            InstructorFullName = null,
            InstructorEmail = null,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            IsPublished = course.Status == Domain.Enums.CourseStatus.Published,
            Prerequisites = course.Prerequisites.Select(p => p.Value).ToArray(),
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Modules = new List<ModuleResponse>() // map modules if available
        };
    }
}
