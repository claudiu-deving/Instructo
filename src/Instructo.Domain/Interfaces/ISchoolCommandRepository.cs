using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface ISchoolCommandRepository : ICommandRepository<School, Guid>
{
    Task<Result<School>> SetApprovalState(Guid id, bool isApproved);
}