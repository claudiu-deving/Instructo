using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Common;
using Instructo.Domain.Entities;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Interfaces;

public interface ICommandRepository<TDomain, TDomainKey>
    where TDomain : IEntity<TDomainKey>
    where TDomainKey : class, IComparable
{
    Task<Result<string>> AddAsync(TDomain entity);
    Task<Result<string>> UpdateAsync(TDomain entity);
    Task<Result<string>> DeleteAsync(TDomain entity);
}