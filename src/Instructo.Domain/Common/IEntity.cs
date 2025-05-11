namespace Domain.Common;

public interface IEntity<T> : IEntity where T : notnull
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    T Id { get; }

    void AddDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    void RemoveDomainEvent(BaseEvent domainEvent);
}

public interface IEntity
{
}