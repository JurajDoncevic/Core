namespace Domain;

/// <summary>
/// Base class for entities.
/// </summary>
public abstract class Entity<TKey>
    where TKey : notnull
{
    public TKey Id => _id;
    protected TKey _id;

    protected Entity(TKey id)
    {
        _id = id;
    }
}
