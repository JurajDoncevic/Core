using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with read functionality for an aggregate.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model representing the aggregate.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IReadAggregateRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : AggregateRoot<TKey>
    where TError : Error
{
    /// <summary>
    /// Gets an aggregate by its root identifier.
    /// </summary>
    /// <param name="id">The identifier of the aggregate root.</param>
    /// <returns>Asynchronous task result containing the aggregate.</returns>
    Task<Result<TDomainModel, TError>> GetAggregateById(TKey id);

    /// <summary>
    /// Gets all aggregates.
    /// </summary>
    /// <returns>Asynchronous task result containing the aggregates.</returns>
    Task<Result<IEnumerable<TDomainModel>, TError>> GetAllAggregates();
}
