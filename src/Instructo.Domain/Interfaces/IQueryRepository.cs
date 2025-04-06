using Instructo.Domain.Common;
using Instructo.Domain.Shared;

namespace Instructo.Domain.Interfaces;

public interface IQueryRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : notnull
{
    Task<Result<TDomain?>> GetByIdAsync(TDomainKey id);
    Task<Result<IEnumerable<TDomain>?>> GetAllAsync();
    Task<Result<IEnumerable<TDomain>?>> GetByIndexed(string indexValue);
}