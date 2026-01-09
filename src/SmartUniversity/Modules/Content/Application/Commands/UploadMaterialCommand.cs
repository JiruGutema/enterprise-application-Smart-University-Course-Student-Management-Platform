using MediatR;
using Microsoft.AspNetCore.Http;
using SmartUniversity.Modules.Content.API.DTOs;

namespace SmartUniversity.Modules.Content.Application.Commands;

public class UploadMaterialCommand : IRequest<MaterialDto>
{
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IFormFile File { get; set; } = null!;
    public Guid UploadedByUserId { get; set; }
}