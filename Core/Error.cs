namespace Core;

public class Error
{
    private readonly string _message;
    private readonly Exception? _exception;
    public string Message => _message;
    public bool IsException => _exception != null;
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

    public static Error FromException(Exception exception)
        => new Error(exception.Message, exception);

    public static Error FromLogical(string message)
        => new Error(message);
}