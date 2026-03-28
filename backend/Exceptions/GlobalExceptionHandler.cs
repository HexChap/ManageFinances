using Microsoft.AspNetCore.Diagnostics;

namespace backend.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is NotFoundException notFound)
        {
            _logger.LogWarning("Resource not found: {Message}", notFound.Message);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new { error = notFound.Message }, cancellationToken);
            return true;
        }

        _logger.LogError(exception, "Unhandled exception");
        return false;
    }
}
