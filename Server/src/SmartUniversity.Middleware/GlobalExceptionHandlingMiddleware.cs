using SmartUniversity.Modules.Identity.Application.Exceptions;
using SmartUniversity.Modules.Identity.Domain.Exceptions;
using SmartUniversity.Modules.Identity.Infrastructure.Exceptions;

namespace SmartUniversity.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await WriteError(context, ex.Message);
            }
            catch (AppException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                await WriteError(context, ex.Message);
            }
            catch (InfrastructureException ex)
            {
                _logger.LogError(ex, "Infrastructure failure");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteError(context, "Service temporarily unavailable");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteError(context, "Unexpected error occurred");
            }
        }

        private static async Task WriteError(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }

        private static int MapApplicationException(AppException ex)
        {
            return ex switch
            {
                _ => StatusCodes.Status400BadRequest,
            };
        }
    }
}
