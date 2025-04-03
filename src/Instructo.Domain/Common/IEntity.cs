
namespace Instructo.Domain.Common;

public interface IEntity<T> where T : class, IComparable
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    T Id { get; }

    void AddDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    void RemoveDomainEvent(BaseEvent domainEvent);
}