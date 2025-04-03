using Instructo.Domain.Common;

namespace Instructo.Domain.Interfaces;

public interface IQueryRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : class, IComparable
{
    Task<TDomain> GetByIdAsync(TDomainKey id);
    Task<IEnumerable<TDomain>> GetAllAsync();
}