using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.AI.Application.DTOS;
using SmartUniversity.Modules.AI.Application.Interfaces;

namespace SmartUniversity.Modules.AI.Api;

[Authorize]
[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("ask")]
    public async Task<ActionResult<AskAIResponse>> Ask([FromBody] AskAIRequest request)
    {
        var userId = GetUserId();
        var result = await _aiService.AskAIAsync(userId, request);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<ChatHistoryDto>>> GetHistory()
    {
        var userId = GetUserId();
        var result = await _aiService.GetHistoryAsync(userId);
        return Ok(result);
    }

    [HttpDelete("history/{id}")]
    public async Task<IActionResult> DeleteHistory(Guid id)
    {
        var userId = GetUserId();
        await _aiService.DeleteHistoryAsync(userId, id);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

        if (idClaim == null)
            throw new UnauthorizedAccessException("User ID not found in token");

        if (Guid.TryParse(idClaim.Value, out var guid))
            return guid;

        throw new UnauthorizedAccessException("Invalid User ID format");
    }
}
