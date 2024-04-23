namespace Core;

/// <summary>
/// Terminal type.
/// </summary>
public struct Unit { };


/// <summary>
/// Unit extension methods.
/// </summary>
public static class UnitExt
{
    /// <summary>
    /// Unit value.
    /// </summary>
    public static Unit Unit() => default(Unit);

    /// <summary>
    /// Ignore value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_"></param>
    /// <returns>A Unit value</returns>
    public static Unit Ignore<T>(this T target) => Unit();

    public static TData Covalesce<TData>(this Unit target)
        => default(TData) ?? typeof(TData) switch
    {
        var type when type == typeof(string) => (TData)(object)string.Empty,
        var type when type == typeof(int) => (TData)(object)0,
        var type when type == typeof(double) => (TData)(object)0.0,
        var type when type == typeof(bool) => (TData)(object)false,
        var type when type == typeof(DateTime) => (TData)(object)DateTime.MinValue,
        var type when type == typeof(Unit) => (TData)(object)Unit(),
        _ => throw new NotSupportedException($"Unsupported type: {typeof(TData)}")
    };
}