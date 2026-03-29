namespace backend.Exceptions;

/// <summary>
/// Thrown by a service when a requested resource does not exist or does not belong to the requesting user.
/// The <see cref="GlobalExceptionHandler"/> converts this to an HTTP 404 response.
/// </summary>
public class NotFoundException : Exception
{
    /// <param name="message">Human-readable description identifying the missing resource.</param>
    public NotFoundException(string message) : base(message) { }
}
