namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class CreateCourseRequest
{
    public string Title { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? InstructorId { get; set; }
    public string[]? Prerequisites { get; set; }
}
