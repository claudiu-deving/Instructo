using Instructo.Domain.Common;
using Instructo.Domain.Shared;

namespace Instructo.Domain.Interfaces;

public interface ICommandRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : struct
{
    Task<Result<TDomain>> AddAsync(TDomain entity);
    Task<Result<TDomain>> UpdateAsync(TDomain entity);
    Task<Result<TDomain>> DeleteAsync(TDomain entity);
}