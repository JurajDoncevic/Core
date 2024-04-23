using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with read functionality.
/// </summary>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IReadRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : Entity<TKey>
    where TError : Error
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>Asynchronous task result containing the entity.</returns>
    Task<Result<TDomainModel, TError>> GetById(TKey id);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>Asynchronous task result containing the entities.</returns>
    Task<Result<IEnumerable<TDomainModel>, TError>> GetAll();
}
