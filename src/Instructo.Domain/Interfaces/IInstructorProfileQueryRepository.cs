
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Shared;

namespace Infrastructure.Data.Repositories.Queries;
public interface IInstructorProfileQueryRepository : IQueryRepository<InstructorProfile, Guid>
{
    Task<Result<IEnumerable<InstructorProfile>>> GetBySchoolIdAsync(Guid schoolId);
}