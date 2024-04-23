using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with update functionality for an aggregate.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IUpdateAggregateRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : AggregateRoot<TKey>
    where TError : Error
{
    /// <summary>
    /// Updates an aggregate.
    /// </summary>
    /// <param name="id">The identifier of the aggregate.</param>
    /// <param name="entity">The aggregate to update.</param>
    /// <returns>Asynchronous task result containing the result of the update operation.</returns>
    Task<Result<TDomainModel, TError>> UpdateAggregate(TKey id, TDomainModel entity);
}
