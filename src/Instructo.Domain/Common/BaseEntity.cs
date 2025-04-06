using System.ComponentModel.DataAnnotations.Schema;

namespace Instructo.Domain.Common;

public abstract class BaseEntity<T> : IEntity<T> where T : notnull
{
    private T _id = default!;
    public T Id
    {
        get => _id;
        // This protected setter allows EF Core to set the ID after generation
        protected set => _id=value;
    }


    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}