
using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;
public interface ISchoolQueriesRepository : IQueryRepository<School, SchoolId>
{
    Task<Result<School?>> GetBySlugAsync(string slug);
}