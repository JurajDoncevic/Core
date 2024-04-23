using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with insert functionality.
/// </summary>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IInsertRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : Entity<TKey>
    where TError : Error
{
    /// <summary>
    /// Inserts an entity.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>Asynchronous task result containing the result of the insert operation.</returns>
    Task<Result<TDomainModel, TError>> Insert(TDomainModel entity);
}
