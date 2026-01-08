namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class CreateLessonRequest
{
    public string Name { get; set; } = null!;
    public string? Content { get; set; }
    public int Order { get; set; }
}