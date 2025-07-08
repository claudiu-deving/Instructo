using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface ISchoolCommandRepository : ICommandRepository<School, SchoolId>
{
    Task<Result<School>> SetApprovalState(SchoolId id, bool isApproved);
}