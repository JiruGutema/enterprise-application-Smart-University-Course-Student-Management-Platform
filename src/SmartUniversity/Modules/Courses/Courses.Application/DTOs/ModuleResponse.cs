namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class ModuleResponse
{
    public Guid ModuleId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Order { get; set; }
}
