using Domain.Common;
using Domain.Shared;

namespace Domain.Interfaces;

public interface ICommandRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : struct
{
    public Task<Result<TDomain>> AddAsync(TDomain entity);
    public Task<Result<TDomain>> UpdateAsync(TDomain entity);
    public Task<Result<TDomain>> DeleteAsync(TDomain entity);
}