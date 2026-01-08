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

    /// <summary>
    /// Student enrolls in a published course. Prerequisites are validated synchronously.
    /// </summary>
    /// <param name="request">Course ID to enroll in</param>
   

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

/// <summary>
/// Get enrollment details by ID. 
/// Students can only view their own enrollments. 
/// Instructors can view enrollments for their courses. 
/// Admin can view any enrollment.
/// </summary>
/// <param name="id">Enrollment ID</param>

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

/// <summary>
/// Get enrollments for the logged-in student.
/// Optional status filter and pagination can be applied.
/// </summary>
/// <param name="request">Filter and pagination parameters</param>


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

/// <summary>
/// Drop an enrollment.
/// Students can drop their own enrollments. Admin can drop any enrollment.
/// </summary>
/// <param name="id">Enrollment ID</param>

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

/// <summary>
/// List students enrolled in a course.
/// Only the assigned instructor or admin can access.
/// Optional status filter and pagination can be applied.
/// </summary>
/// <param name="courseId">Course ID</param>
/// <param name="status">Filter by enrollment status</param>
/// <param name="page">Page number</param>
/// <param name="pageSize">Page size</param>
/// <returns>List of students in the course</returns>
[ProducesResponseType(typeof(CourseEnrollmentStudentsResponse), 200)]
[ProducesResponseType(403)]



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

/// <summary>
/// Admin search of all enrollments with optional filters for student, course, and status.
/// </summary>
/// <param name="studentId">Filter by student ID</param>
/// <param name="courseId">Filter by course ID</param>
/// <param name="status">Filter by enrollment status</param>
/// <param name="page">Page number</param>
/// <param name="pageSize">Page size</param>
/// <returns>Paginated list of enrollments</returns>
[ProducesResponseType(typeof(AdminEnrollmentsResponse), 200)]
[ProducesResponseType(403)]

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
/// <summary>
/// Change enrollment status (Admin only).
/// </summary>
/// <param name="id">Enrollment ID</param>
/// <param name="request">New status</param>


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
  /// <summary>
/// Internal endpoint to validate student prerequisites for a course (Admin only).
/// </summary>
/// <param name="studentId">Student ID</param>
/// <param name="courseId">Course ID</param>


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
