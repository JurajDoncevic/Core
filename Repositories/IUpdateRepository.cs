using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with update functionality.
/// </summary>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IUpdateRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : Entity<TKey>
    where TError : Error
{
    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="entity">The entity to update.</param>
    /// <returns>Asynchronous task result containing the result of the update operation.</returns>
    Task<Result<TDomainModel, TError>> Update(TKey id, TDomainModel entity);
}
