using Microsoft.AspNetCore.Mvc;
using MediatR;
using SmartUniversity.Modules.Courses.Application.Commands;
using SmartUniversity.Modules.Courses.Application.Queries;
using SmartUniversity.Modules.Courses.Application.DTOs;

namespace SmartUniversity.Modules.Courses.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses Context")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/courses
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var courseId = await _mediator.Send(new CreateCourseCommand(
            request.Title,
            request.Code,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.InstructorId ?? Guid.Empty,
            request.Prerequisites?.ToList()
        ));

        return CreatedAtAction(nameof(GetById), new { courseId }, new { CourseId = courseId });
    }

    // GET /api/courses
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] bool publishedOnly = true,
        [FromQuery] Guid? instructorId = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeUnpublished = false
    )
    {
        var result = await _mediator.Send(new GetCoursesQuery
        {
            PublishedOnly = publishedOnly,
            InstructorId = instructorId,
            Search = search,
            Page = page,
            PageSize = pageSize,
            IncludeUnpublished = includeUnpublished
        });

        return Ok(result);
    }

    // GET /api/courses/{courseId}
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetById(Guid courseId)
    {
        var course = await _mediator.Send(new GetCourseByIdQuery(courseId));
        if (course == null)
            return NotFound(new { error = "Course not found" });

        return Ok(course);
    }

    // PUT /api/courses/{courseId}
    [HttpPut("{courseId}")]
    public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] UpdateCourseRequest request)
    {
        await _mediator.Send(new UpdateCourseCommand(
            courseId,
            request.Title,
            request.Code,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.InstructorId,
            request.Prerequisites
        ));

        return Ok();
    }

    // PATCH /api/courses/{courseId}/publish
    [HttpPatch("{courseId}/publish")]
    public async Task<IActionResult> Publish(Guid courseId)
    {
        await _mediator.Send(new PublishCourseCommand(courseId));
        return NoContent();
    }

    // PATCH /api/courses/{courseId}/unpublish
    [HttpPatch("{courseId}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid courseId)
    {
        await _mediator.Send(new UnpublishCourseCommand(courseId));
        return NoContent();
    }

    // DELETE /api/courses/{courseId}
    [HttpDelete("{courseId}")]
    public async Task<IActionResult> DeleteCourse(Guid courseId)
    {
        await _mediator.Send(new DeleteCourseCommand(courseId));
        return NoContent();
    }

    // Internal endpoints for Enrollment / other contexts
    [HttpGet("internal/{courseId}/metadata")]
    public async Task<IActionResult> GetInternalMetadata(Guid courseId)
    {
        var metadata = await _mediator.Send(new GetCourseMetadataQuery(courseId));
        return Ok(metadata);
    }

    [HttpGet("internal/search-by-codes")]
    public async Task<IActionResult> SearchByCodes([FromQuery] string codes)
    {
        var courseList = await _mediator.Send(new GetCoursesByCodesQuery(codes.Split(',')));
        return Ok(courseList);
    }
}
