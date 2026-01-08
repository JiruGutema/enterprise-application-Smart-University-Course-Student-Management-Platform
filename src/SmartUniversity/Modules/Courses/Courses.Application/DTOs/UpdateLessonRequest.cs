namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class UpdateLessonRequest
{
    public string? Name { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
}