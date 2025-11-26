namespace DocumentManagement.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; init; }

    protected Entity(TId id)
    {
        Id = id;
    }
}
