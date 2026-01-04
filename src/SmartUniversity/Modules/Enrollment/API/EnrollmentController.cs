using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Enrollment.Application.Commands;

namespace SmartUniversity.Modules.Enrollment.Api;

[ApiController]
[Route("api/enrollments")]
// [Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(EnrollStudentCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Drop(Guid id)
    {
        await _mediator.Send(new DropEnrollmentCommand(id));
        return NoContent();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(new { EnrollmentId = id });
    }
}
