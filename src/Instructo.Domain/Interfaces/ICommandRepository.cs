using Domain.Common;
using Domain.Shared;

namespace Domain.Interfaces;

public interface ICommandRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : struct
{
    Task<Result<TDomain>> AddAsync(TDomain entity);
    Task<Result<TDomain>> UpdateAsync(TDomain entity);
    Task<Result<TDomain>> DeleteAsync(TDomain entity);
}