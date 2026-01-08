namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class CreateModuleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Order { get; set; }
}