namespace Core;

/// <summary>
/// Error type for <see cref="Result"/>
/// </summary>
public class Error
{
    private readonly string _message;
    private readonly Exception? _exception;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Is there an exception that caused the error
    /// </summary>
    public bool IsException => _exception != null;
    /// <summary>
    /// Exception that caused the error
    /// </summary>
    public Exception Exception => _exception!;

    protected Error(string? message = "Operation failed", Exception? exception = null)
    {
        _message = message ?? "Operation failed";
        _exception = exception;
        if (exception != null)
        {
            _message += $": {exception.Message}";
        }
    }

    public static implicit operator Error(Exception exception)
        => new Error(exception.Message, exception);

    /// <summary>
    /// Creates an error from an exception
    /// </summary>
    /// <param name="exception">Exception that caused the error</param>
    /// <returns>Error with exception</returns>
    public static Error FromException(Exception exception)
        => new Error(exception.Message, exception);

    /// <summary>
    /// Creates an (logical) error from a message
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Logical error</returns>
    public static Error FromLogical(string message)
        => new Error(message);
}