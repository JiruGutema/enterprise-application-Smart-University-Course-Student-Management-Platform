namespace SmartUniversity.Modules.Courses.Application.DTOs;

public class UpdateModuleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}