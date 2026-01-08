namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class LessonResponse
{
    public Guid LessonId { get; set; }
    public string Name { get; set; } = null!;
    public string? Content { get; set; }
    public int Order { get; set; }
}