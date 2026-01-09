using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Content.API.DTOs;
using SmartUniversity.Modules.Content.Application.Commands;
using SmartUniversity.Modules.Content.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace SmartUniversity.Modules.Content.API;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("courses/{courseId}/materials")]

    public async Task<IActionResult> UploadMaterial(Guid courseId, [FromForm] MaterialUploadRequest request)
    {
        var command = new UploadMaterialCommand
        {
            CourseId = courseId,
            File = request.File,
            Title = request.Title,
            Description = request.Description,
            LessonId = Guid.TryParse(request.LessonId, out var lessonId) ? lessonId : null,
            UploadedByUserId = Guid.TryParse(request.UploadedByUserId, out var userId) ? userId : Guid.Empty
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMaterialById), new { materialId = result.MaterialId }, result);
    }
    [Authorize]
    [HttpGet("courses/{courseId}/materials")]
    public async Task<IActionResult> GetMaterialsByCourse(
         Guid courseId,
         [FromQuery] Guid? lessonId,
         [FromQuery] string? fileType,
         [FromQuery] string? search,
         [FromQuery] string? sort,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 20)
    {
        var query = new GetMaterialsByCourseQuery
        {
            CourseId = courseId,
            LessonId = lessonId,
            FileType = fileType,
            Search = search,
            Sort = sort,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("materials/{materialId}")]
    public async Task<IActionResult> GetMaterialById(Guid materialId)
    {
        var query = new GetMaterialByIdQuery { MaterialId = materialId };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpGet("materials/{materialId}/download")]
    public async Task<IActionResult> DownloadMaterial(Guid materialId)
    {
        var query = new GetMaterialByIdQuery { MaterialId = materialId };
        var material = await _mediator.Send(query);

        if (material == null || !System.IO.File.Exists(material.FilePath))
            return NotFound("File not found");

        var fileStream = System.IO.File.OpenRead(material.FilePath);
        return File(fileStream, "application/octet-stream", material.FileName);
    }
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("materials/{materialId}")]
    public async Task<IActionResult> UpdateMaterial(Guid materialId, [FromBody] UpdateMaterialRequest request)
    {
        var command = new UpdateMaterialCommand
        {
            MaterialId = materialId,
            Title = request.Title,
            Description = request.Description,
            LessonId = request.LessonId
        };

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("materials/{materialId}")]
    public async Task<IActionResult> DeleteMaterial(Guid materialId)
    {
        var command = new DeleteMaterialCommand { MaterialId = materialId };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }
}