using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

public abstract class BaseEntity<T> : IEntity<T> where T : notnull
{
    private readonly List<BaseEvent> _domainEvents = [];

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id
    {
        get;
        // This protected setter allows EF Core to set the ID after generation
        protected set;
    } = default!;

    [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

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