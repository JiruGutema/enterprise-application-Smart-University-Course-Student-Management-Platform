using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using SmartUniversity.Modules.Enrollment.Application.Queries;
using SmartUniversity.Modules.Enrollment.Api.DTOs;
using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        string? studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid studentId = Guid.Parse(studentIdClaim!);

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

  [Authorize]
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Guid userId = Guid.Parse(userIdClaim!);

    var roles = User.Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();

    var enrollment = await _mediator.Send(
        new GetEnrollmentByIdQuery(id, userId, roles)
    );

    if (enrollment == null)
        return NotFound();

    return Ok(enrollment);
}



 [Authorize(Roles = "Student")]
[HttpGet("my")]
public async Task<IActionResult> GetMyEnrollments([FromQuery] GetMyEnrollmentsRequest request)
{
    string? studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Guid studentId = Guid.Parse(studentIdClaim!);

    var result = await _mediator.Send(new GetMyEnrollmentsQuery
    {
        StudentId = studentId,      
        Status = request.Status,
        Page = request.Page,
        PageSize = request.PageSize
    });

    return Ok(result);
}

// PATCH /api/enrollments/{enrollmentId}/drop
[Authorize(Roles = "Student,Admin")]
[HttpPatch("{id}/drop")]
public async Task<IActionResult> Drop(Guid id)
{
    string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Guid userId = Guid.Parse(userIdClaim!);

    bool isAdmin = User.IsInRole("Admin");

    await _mediator.Send(new DropEnrollmentCommand(id, userId, isAdmin));

    return NoContent();
}

// GET /api/enrollments/courses/{courseId}/students 
//  will add // [Authorize(Roles = "Instructor,Admin")]

[Authorize(Roles = "Instructor,Admin")]
[HttpGet("courses/{courseId}/students")]
public async Task<IActionResult> GetStudentsByCourse(
    Guid courseId,
    [FromQuery] string? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)
{
    string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Guid instructorId = Guid.Parse(userIdClaim!);

    bool isAdmin = User.IsInRole("Admin");
    var result = await _mediator.Send(new GetStudentsByCourseQuery
    {
        CourseId = courseId,
        Status = status,
        Page = page,
        PageSize = pageSize  
    });

    return Ok(result);
}

// GET /api/enrollments/admin
[Authorize(Roles = "Admin")]
[HttpGet("admin")]
public async Task<IActionResult> AdminSearchEnrollments(
    [FromQuery] Guid? studentId,
    [FromQuery] Guid? courseId,
    [FromQuery] string? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    var result = await _mediator.Send(new AdminSearchEnrollmentsQuery
    {
        StudentId = studentId,
        CourseId = courseId,
        Status = status,
        Page = page,
        PageSize = pageSize
    });

    return Ok(result);
}


// PATCH /api/enrollments/{enrollmentId}/status
[Authorize(Roles = "Admin")]
[HttpPatch("{id}/status")]
public async Task<IActionResult> ChangeStatus(
    Guid id,
    [FromBody] ChangeEnrollmentStatusRequest request)
{
    await _mediator.Send(new ChangeEnrollmentStatusCommand(id, request.Status));
    return NoContent();
}


  // GET /api/enrollments/internal/validate-prerequisites
  [Authorize(Roles = "Admin")]
    [HttpGet("internal/validate-prerequisites")]
    public IActionResult ValidatePrerequisites([FromQuery] Guid studentId, [FromQuery] Guid courseId)
    {
        // TEMP: mock logic for now
        var response = new PrerequisiteValidationResponse
        {
            IsEligible = true,
            MissingPrerequisites = new List<string>() // empty list = all prerequisites met
        };

        return Ok(response);
    }
}
