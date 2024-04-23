using Core;
using Domain;

namespace Respositories;

/// <summary>
/// Interface for a repository with insert functionality for an aggregate.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root's identifier.</typeparam>
/// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
/// <typeparam name="TError">The type of error that can occur.</typeparam>
public interface IInsertAggregateRepository<TKey, TDomainModel, TError>
    where TKey : notnull
    where TDomainModel : AggregateRoot<TKey>
    where TError : Error
{
    /// <summary>
    /// Inserts an aggregate.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate to insert.</param>
    /// <returns>Asynchronous task result containing the result of the insert operation.</returns>
    Task<Result<TDomainModel, TError>> InsertAggregate(TDomainModel agggregateRoot);
}
