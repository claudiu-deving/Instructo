
using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;
public interface ISchoolQueriesRepository
{
    Task<Result<IEnumerable<School>?>> GetAllAsync();
    Task<Result<School?>> GetByIdAsync(SchoolId id);
    Task<Result<School?>> GetByIndexed(string companyName);
    Task<Result<School?>> GetBySlugAsync(string slug);
}