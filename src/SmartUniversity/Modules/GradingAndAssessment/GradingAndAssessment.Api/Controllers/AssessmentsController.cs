using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.GradingAndAssessment.Application.Commands;
using SmartUniversity.Modules.GradingAndAssessment.Application.DTOs;
using SmartUniversity.Modules.GradingAndAssessment.Application.Queries;

namespace SmartUniversity.Modules.GradingAndAssessment.Api.Controllers;

[ApiController]
[Route("api/assessments")]
public class AssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssessmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create Assignment
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("courses/{courseId}/assignments")]
    public async Task<IActionResult> CreateAssignment([FromRoute] Guid courseId, [FromBody] CreateAssignmentRequest request)
    {
        var command = new CreateAssignmentCommand(
            courseId,
            request.Title,
            request.Description,
            request.Type,
            request.DueDate,
            request.MaxScore,
            request.Weight
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(CreateAssignment), new { data = result });
    }

    /// <summary>
    /// List Assignments for a Course
    /// </summary>
    [Authorize]
    [HttpGet("courses/{courseId}/assignments")]
    public async Task<IActionResult> GetAssignments([FromRoute] Guid courseId)
    {
        var query = new GetAssignmentsByCourseQuery(courseId);
        var result = await _mediator.Send(query);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Update Assignment
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("assignments/{assignmentId}")]
    public async Task<IActionResult> UpdateAssignment([FromRoute] Guid assignmentId, [FromBody] UpdateAssignmentRequest request)
    {
        var command = new UpdateAssignmentCommand(
            assignmentId,
            request.Title,
            request.Description,
            request.DueDate,
            request.MaxScore,
            request.Weight
        );

        var result = await _mediator.Send(command);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Delete Assignment
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("assignments/{assignmentId}")]
    public async Task<IActionResult> DeleteAssignment([FromRoute] Guid assignmentId)
    {
        var command = new DeleteAssignmentCommand(assignmentId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Record or Update Grade
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("assignments/{assignmentId}/students/{studentId}/grade")]
    public async Task<IActionResult> RecordGrade([FromRoute] Guid assignmentId, [FromRoute] Guid studentId, [FromBody] RecordGradeRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var command = new RecordGradeCommand(
            assignmentId,
            studentId,
            request.Score,
            request.Feedback,
            Guid.Parse(userId!)
        );

        var result = await _mediator.Send(command);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Bulk Grading
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("assignments/{assignmentId}/grades/bulk")]
    public async Task<IActionResult> BulkRecordGrades([FromRoute] Guid assignmentId, [FromBody] List<BulkGradeRequest> request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var command = new BulkRecordGradesCommand(assignmentId, request, Guid.Parse(userId!));

        var result = await _mediator.Send(command);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Get My Assignments for a Course
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpGet("my/courses/{courseId}/assignments")]
    public async Task<IActionResult> GetMyAssignments([FromRoute] Guid courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = new GetStudentAssignmentsQuery(courseId, Guid.Parse(userId!));
        var result = await _mediator.Send(query);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Get My Overall Grade in a Course
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpGet("my/courses/{courseId}/summary")]
    public async Task<IActionResult> GetMyGradeSummary([FromRoute] Guid courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = new GetStudentGradeSummaryQuery(courseId, Guid.Parse(userId!));
        var result = await _mediator.Send(query);
        return Ok(new { data = result });
    }

    /// <summary>
    /// Gradebook for a Course
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("courses/{courseId}/gradebook")]
    public async Task<IActionResult> GetGradebook([FromRoute] Guid courseId)
    {
        var query = new GetGradebookQuery(courseId);
        var result = await _mediator.Send(query);
        return Ok(new { data = result });
    }
}