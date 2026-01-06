using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using MediatR;

namespace SmartUniversity.Modules.Enrollment.Api;

[ApiController]
[Route("api/enrollments")]
[Tags("Enrollment Context")]
public class EnrollmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
   
        var studentId = request.StudentId;

        var enrollmentId = await _mediator.Send(
            new EnrollStudentCommand(studentId, request.CourseId)
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = enrollmentId },
            new EnrollmentResponse(
                EnrollmentId: enrollmentId,
                CourseId: request.CourseId,
                StudentId: studentId,
                EnrollmentDate: DateTime.UtcNow,
                Status: Domain.Enums.EnrollmentStatus.Enrolled,
                ProgressPercentage: 0
            )
        );
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        // Placeholder
        return Ok(new { EnrollmentId = id });
    }

   [HttpGet("my")]
public async Task<IActionResult> GetMyEnrollments([FromQuery] GetMyEnrollmentsRequest request)
{
    var studentId = request.StudentId; // will read from JWT later

    var result = await _mediator.Send(new GetMyEnrollmentsQuery
    {
        StudentId = studentId,
        Status = request.Status,
        Page = request.Page,
        PageSize = request.PageSize
    });

    return Ok(result);
}




}
