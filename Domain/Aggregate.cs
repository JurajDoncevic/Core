namespace Domain;

/// <summary>
/// Base class for aggregate roots.
/// </summary>
public abstract class AggregateRoot<TKey> 
    : Entity<TKey>
    where TKey : notnull
{
    protected AggregateRoot(TKey id) : base(id)
    {
    }
}