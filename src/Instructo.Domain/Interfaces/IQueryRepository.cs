using Domain.Common;
using Domain.Shared;

namespace Domain.Interfaces;

public interface IQueryRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : notnull
{
    Task<Result<TDomain?>> GetByIdAsync(TDomainKey id);
    Task<Result<IEnumerable<TDomain>?>> GetAllAsync();
    Task<Result<IEnumerable<TDomain>?>> GetByIndexed(string indexValue);
}