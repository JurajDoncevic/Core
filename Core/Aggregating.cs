namespace Core;

public static class Aggregating
{
    /// <summary>
    /// Used to fold a IEnumerable into a single value.
    /// E[T] -> R -> (T -> R -> R) -> R
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="ts">Target enumerable</param>
    /// <param name="initalizer">Initial value</param>
    /// <param name="foldingFunc">Accumulation function</param>
    /// <returns></returns>
    public static R Fold<T, R>(this IEnumerable<T> ts, R seed, Func<T, R, R> foldingFunc)
    {
        R result = seed;
        for (int i = 0;  i < ts.Count(); i++)
        {
            result = foldingFunc(ts.ElementAt(i), result);
        }

        return result;
    }

    /// <summary>
    /// Used to async fold a IEnumerable into a single value with element index as parameter to folding func.
    /// E[T] -> R -> (T -> R -> R) -> R
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="ts">Target enumerable</param>
    /// <param name="initalizer">Initial value</param>
    /// <param name="foldingFunc">Accumulation function</param>
    /// <returns></returns>
    public static R Foldi<T, R>(this IEnumerable<T> ts, R seed, Func<int, T, R, R> foldingFunc)
    {
        R result = seed;
        for (int i = 0; i < ts.Count(); i++)
        {
            result = foldingFunc(i, ts.ElementAt(i), result);
        }

        return result;
    }

    /// <summary>
    /// Used to async fold a IEnumerable into a single value.
    /// E[T] -> R -> (T -> R -> R) -> R
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="ts">Target enumerable</param>
    /// <param name="initalizer">Initial value</param>
    /// <param name="foldingFunc">Accumulation function</param>
    /// <returns></returns>
    public static Task<R> Fold<T, R>(this Task<IEnumerable<T>> ts, R seed, Func<T, R, R> foldingFunc)
    {
        return ts.ContinueWith(t => t.Result.Fold(seed, foldingFunc));
    }

    /// <summary>
    /// Used to async fold a IEnumerable into a single value with element index as parameter to folding func.
    /// E[T] -> R -> (T -> R -> R) -> R
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="ts">Target enumerable</param>
    /// <param name="initalizer">Initial value</param>
    /// <param name="foldingFunc">Accumulation function</param>
    /// <returns></returns>
    public static Task<R> Foldi<T, R>(this Task<IEnumerable<T>> ts, R seed, Func<int, T, R, R> foldingFunc)
    {
        return ts.ContinueWith(t => t.Result.Foldi(seed, foldingFunc));
    }
}