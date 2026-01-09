using MediatR;
using SmartUniversity.Modules.Content.API.DTOs;

namespace SmartUniversity.Modules.Content.Application.Commands;

public class UpdateMaterialCommand : IRequest<MaterialDto?>
{
    public Guid MaterialId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? LessonId { get; set; }
}