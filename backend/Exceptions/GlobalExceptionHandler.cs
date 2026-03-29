using Microsoft.AspNetCore.Diagnostics;

namespace backend.Exceptions;

/// <summary>
/// Global exception handler registered in the middleware pipeline.
/// Maps known domain exceptions to appropriate HTTP responses so controllers
/// do not need individual try/catch blocks.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <param name="logger">Logger used to record warning and error entries.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Attempts to handle the exception and write a JSON error response.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the exception was handled (response written);
    /// <c>false</c> to let the next handler in the pipeline process it.
    /// </returns>
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
