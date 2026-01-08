using System.ComponentModel.DataAnnotations;

namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class UpdateCourseRequest
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    // Only Admin can override this
    public Guid? InstructorId { get; set; }

    // Optional, list of prerequisite course codes
    public List<string>? Prerequisites { get; set; }
}
