namespace Core;

/// <summary>
/// Result monad with either some Data or some Error. Error must be a subclass of <see cref="Error"/>
/// </summary>
/// <typeparam name="TData">Possible data type as result</typeparam>
/// <typeparam name="TError">Possible error type as result</typeparam>
public sealed class Result<TData, TError>
    where TError : Error
{
    private readonly TData? _data;
    private readonly string _message;
    private readonly TError? _error;
    private readonly bool _isSuccess;

    /// <summary>
    /// Is the operation a succcess?
    /// </summary>
    public bool IsSuccess => _isSuccess;
    /// <summary>
    /// Is the operation a failure?
    /// </summary>
    public bool IsFailure => !_isSuccess;
    /// <summary>
    /// Result data - don't use unless <c>IsSuccess</c>
    /// </summary>
    public TData Data => _data!;
    /// <summary>
    /// Thrown exception - don't use unless <c>IsFailure</c>
    /// </summary>
    public TError Error => _error!;
    /// <summary>
    /// Result message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">Possible data</param>
    /// <param name="isSuccess">Operation outcome</param>
    /// <param name="message">Result message override</param>
    /// <param name="error">Possible error</param>
    internal Result(TData? data, bool isSuccess, string? message = null, TError? error = null)
    {
        _data = data;
        _error = error;
        _isSuccess = data != null ? isSuccess : false;
        _message = _isSuccess
            ? message ?? "Operation succeeded"
            : error?.Message ?? message ?? "Operation failed";
    }

    /// <summary>
    /// Implicit bool operator
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator bool(Result<TData, TError> result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a value into a Result
    /// </summary>
    /// <param name="data"></param>
    public static implicit operator Result<TData, TError>(TData data)
        => data != null
            ? Result.Success<TData, TError>(data)
            : Result.Failure<TData, TError>();

    /// <summary>
    /// Implicit operator to turn an error into a Result
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator Result<TData, TError>(TError error)
        => Result.Failure<TData, TError>(error);

    public override bool Equals(object? obj)
    {
        return obj is Result<TData, TError> result &&
               EqualityComparer<TData?>.Default.Equals(_data, result._data) &&
                _message.Equals(result._message) &&
                _error == result._error;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_data, _error);
    }

    public override string ToString()
        => IsSuccess
            ? $"Result<{typeof(TData).Name}, {typeof(TError).Name}>: \"{Data}\""
            : $"Result<{typeof(TData).Name}, {typeof(TError).Name}>: Fail with: {Message}";
}

/// <summary>
/// Extension methods for <see cref="Result"/>
/// </summary>
public static class Result
{
    /// <summary>
    /// Create a success result
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="data">Result data</param>
    /// <param name="message">Overrride message</param>
    public static Result<TData, TError> Success<TData, TError>(TData data, string? message = null)
        where TError : Error
        => new(data, true, message);

    /// <summary>
    /// Create a failure result
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="error">Result error</param>
    /// <param name="message">Override message</param>
    public static Result<TData, TError> Failure<TData, TError>(TError? error = null, string? message = null)
        where TError : Error
        => new(default, false, message, error ?? Error.FromLogical(message ?? "Operation failed") as TError);

    /// <summary>
    /// Match a result with a success and error function
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to match</param>
    /// <param name="successFunc">Success function</param>
    /// <param name="errorFunc">Error function</param>
    /// <returns>Convalesced output data</returns>
    public static TOutData Match<TData, TError, TOutData>(
        this Result<TData, TError> target,
        Func<TData, TOutData> successFunc,
        Func<TError, TOutData> errorFunc
    )
        where TError : Error
    {
        if (target.IsSuccess)
        {
            return successFunc(target.Data);
        }
        else
        {
            return errorFunc(target.Error);
        }
    }

    /// <summary>
    /// Match a result with a success and error function asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to match</param>
    /// <param name="successFunc">Success function</param>
    /// <param name="errorFunc">Error function</param>
    /// <returns>Convalesced output data</returns>
    public static Task<TOutData> Match<TData, TError, TOutData>(
        this Task<Result<TData, TError>> target,
        Func<TData, TOutData> successFunc,
        Func<TError, TOutData> errorFunc
    )
        where TError : Error
        => target.ContinueWith(t => t.Result.Match(successFunc, errorFunc));

    /// <summary>
    /// Map a result to a new result with a data map function
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Data map function</param>
    /// <returns>Mapped result</returns>
    public static Result<TOutData, TError> Map<TData, TError, TOutData>(
        this Result<TData, TError> target,
        Func<TData, TOutData> mapFunc
    )
        where TError : Error
        => target.Match(
            data => mapFunc(data),
            error => Result.Failure<TOutData, TError>(error)
        );

    /// <summary>
    /// Map a result to a new result with a data map function asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Data map function</param>
    /// <returns>Mapped result</returns>
    public static Task<Result<TOutData, TError>> Map<TData, TError, TOutData>(
        this Task<Result<TData, TError>> target,
        Func<TData, TOutData> mapFunc
    )
        where TError : Error
        => target.ContinueWith(t => t.Result.Map(mapFunc));


    /// <summary>
    /// Map a Result error to a new error with an error map function
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutError">Type of output error</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Error map function</param>
    /// <returns>Mapped error result</returns>
    public static Result<TData, TOutError> MapError<TData, TError, TOutError>(
        this Result<TData, TError> target,
        Func<TError, TOutError> mapFunc
    )
        where TError : Error
        where TOutError : Error
        => target.Match(
            data => Result.Success<TData, TOutError>(data),
            error => mapFunc(error)
        );

    /// <summary>
    /// Map a Result error to a new error with an error map function asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutError">Type of output error</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Error map function</param>
    /// <returns>Mapped error result</returns>
    public static Task<Result<TData, TOutError>> MapError<TData, TError, TOutError>(
        this Task<Result<TData, TError>> target,
        Func<TError, TOutError> mapFunc
    )
        where TError : Error
        where TOutError : Error
        => target.ContinueWith(t => t.Result.MapError(mapFunc));

    /// <summary>
    /// Bind a result to a new result with a data bind function
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to bind</param>
    /// <param name="bindFunc">Data bind function</param>
    /// <returns>Bound result</returns>
    public static Result<TOutData, TError> Bind<TData, TError, TOutData>(
        this Result<TData, TError> target,
        Func<TData, Result<TOutData, TError>> bindFunc
    )
        where TError : Error
        => target.Match(
            data => bindFunc(data),
            error => Result.Failure<TOutData, TError>(error)
        );

    /// <summary>
    /// Bind a result to a new result with a data bind function asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to bind</param>
    /// <param name="bindFunc">Data bind function</param>
    /// <returns>Bound result</returns>
    public static Task<Result<TOutData, TError>> Bind<TData, TError, TOutData>(
        this Task<Result<TData, TError>> target,
        Func<TData, Result<TOutData, TError>> bindFunc
    )
        where TError : Error
        => target.ContinueWith(t => 
            t.Result.IsSuccess 
            ? t.Result.Bind(bindFunc) 
            : Result.Failure<TOutData, TError>(t.Result.Error)
        );

    /// <summary>
    /// Bind a result to a new result with a data bind function asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutData">Type of output data</typeparam>
    /// <param name="target">Result to bind</param>
    /// <param name="bindFunc">Data bind function</param>
    /// <returns>Bound result</returns>
    public static async Task<Result<TOutData, TError>> Bind<TData, TError, TOutData>(
        this Task<Result<TData, TError>> target,
        Func<TData, Task<Result<TOutData, TError>>> bindFunc
    )
        where TError : Error
    {
        var t = await target;
        return t.IsSuccess 
            ? await bindFunc(t.Data) 
            : Result.Failure<TOutData, TError>(t.Error);
    }

    /// <summary>
    /// Bind a Result error to a new Result asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutError">Type of output error</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Error map function</param>
    /// <returns>Mapped error result</returns>
    /// <returns>Bound result</returns>
    public static Result<TData, TOutError> BindError<TData, TError, TOutError>(
        this Result<TData, TError> target,
        Func<TError, Result<TData, TOutError>> bindFunc
    )
        where TError : Error
        where TOutError : Error
        => target.Match(
            data => Result.Success<TData, TOutError>(data),
            error => bindFunc(error)
    );

    /// <summary>
    /// Bind a Result error to a new Result asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutError">Type of output error</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Error map function</param>
    /// <returns>Mapped error result</returns>
    public static Task<Result<TData, TOutError>> BindError<TData, TError, TOutError>(
        this Task<Result<TData, TError>> target,
        Func<TError, Result<TData, TOutError>> bindFunc
    )
        where TError : Error
        where TOutError : Error
        => target.ContinueWith(t => t.Result.BindError(bindFunc));

    /// <summary>
    /// Bind a Result error to a new Result asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <typeparam name="TOutError">Type of output error</typeparam>
    /// <param name="target">Result to map</param>
    /// <param name="mapFunc">Error map function</param>
    /// <returns>Mapped error result</returns>
    public static Task<Result<TData, TOutError>> BindError<TData, TError, TOutError>(
        this Task<Result<TData, TError>> target,
        Func<TError, Task<Result<TData, TOutError>>> bindFunc
    )
        where TError : Error
        where TOutError : Error
        => target.ContinueWith(t => bindFunc(t.Result.Error)).Result;


    /// <summary>
    /// Run an operation and package the result as a <see cref="Result"/>
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <param name="operation">Operation to run</param>
    /// <returns>Result of operation</returns>
    public static Result<TData, Error> AsResult<TData>(Func<Result<TData, Error>> operation)
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            return Result.Failure<TData, Error>(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Run an operation and package the result as a <see cref="Result"/> with exception handling
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="operation">Operation to run</param>
    /// <param name="onError">Exception error function</param>
    /// <returns>Result of operation</returns>
    public static Result<TData, TError> AsResult<TData, TError>(Func<Result<TData, TError>> operation, Func<Exception, TError> onError)
        where TError : Error
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            return onError(ex);
        }
    }

    /// <summary>
    /// Run an operation and package the result as a <see cref="Result"/> asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <param name="operation">Operation to run</param>
    /// <returns>Result of operation</returns>
    public static async Task<Result<TData, Error>> AsResult<TData>(Func<Task<Result<TData, Error>>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            return Result.Failure<TData, Error>(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Run an operation and package the result as a <see cref="Result"/> asynchronously with exception handling
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="operation">Operation to run</param>
    /// <param name="onError">Exception error function</param>
    /// <returns>Result of operation</returns>
    public static async Task<Result<TData, TError>> AsResult<TData, TError>(
        Func<Task<Result<TData, TError>>> operation,
        Func<Exception, TError> onError
    )
        where TError : Error
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            return onError(ex);
        }
    }

    /// <summary>
    /// Unfold a list of results into a result of lists
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="results">List of results</param>
    /// <returns>Result of list of data</returns>
    public static Result<IEnumerable<TData>, TError> Unfold<TData, TError>(
        this IEnumerable<Result<TData, TError>> results
    )
        where TError : Error
    {
        if (results.Any(r => r.IsFailure))
        {
            return Result.Failure<IEnumerable<TData>, TError>(results.First(r => r.IsFailure).Error);
        }
        return Result.Success<IEnumerable<TData>, TError>(results.Map(r => r.Data));
    }

    /// <summary>
    /// Unfold a list of results into a result of lists asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="results">List of results</param>
    /// <returns>Result of list of data</returns>
    public static Task<Result<IEnumerable<TData>, TError>> Unfold<TData, TError>(
        this Task<IEnumerable<Result<TData, TError>>> results
    )
        where TError : Error
        => results.ContinueWith(t => t.Result.Unfold());

        /// <summary>
    /// Unfold a list of results into a result of lists asynchronously
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    /// <typeparam name="TError">Type of error</typeparam>
    /// <param name="results">List of results</param>
    /// <returns>Result of list of data</returns>
    public static Task<Result<IEnumerable<TData>, TError>> Unfold<TData, TError>(
        this IEnumerable<Task<Result<TData, TError>>> results
    )
        where TError : Error
        => Task.WhenAll(results).ContinueWith(t => t.Result.Unfold());
}