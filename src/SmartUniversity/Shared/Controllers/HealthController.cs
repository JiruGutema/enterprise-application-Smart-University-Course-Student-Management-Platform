using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;

namespace SmartUniversity.Shared.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly UserDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(UserDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check database connectivity
            await _dbContext.Database.CanConnectAsync();
            
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Database = "Connected",
                Uptime = Environment.TickCount64
            };

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            
            var healthStatus = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message,
                Database = "Disconnected"
            };

            return StatusCode(503, healthStatus);
        }
    }

    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        try
        {
            // More comprehensive readiness check
            await _dbContext.Database.CanConnectAsync();
            
            // Check if migrations are applied
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                return StatusCode(503, new { Status = "Not Ready", Reason = "Pending migrations" });
            }

            return Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");
            return StatusCode(503, new { Status = "Not Ready", Error = ex.Message });
        }
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }
}