using Core;

namespace Repositories;

/// <summary>
/// Interface for a repository with delete functionality.
/// </summary>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IDeleteRepository<TKey, TError>
    where TKey : notnull
    where TError : Error
{
    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <returns>Asynchronous task result containing the result of the delete operation.</returns>
    Task<Result<Unit, TError>> Delete(TKey id);
}
