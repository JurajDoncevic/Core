namespace Core;

/// <summary>
/// Identity functor type
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Identity<T>
{
    private readonly T _data;

    public readonly T Data => _data;

    internal Identity(T data)
    {
        _data = data;
    }
}

public static class IdentityExt
{
    /// <summary>
    /// Creates an identity functor from a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Identity<T> ToIdentity<T>(this T target)
        => new(target);

    /// <summary>
    /// Identity functor map function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="target"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Identity<R> Map<T, R>(this Identity<T> target, Func<T, R> func)
        => new(func(target.Data));
}