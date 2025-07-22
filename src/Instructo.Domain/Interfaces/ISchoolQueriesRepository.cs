
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;
public interface ISchoolQueriesRepository
{
    Task<Result<School?>> GetByIdAsync(SchoolId id);
    Task<Result<bool>> SchoolExists(string companyName);
    Task<Result<SchoolDetailReadDto?>> GetBySlugAsync(string slug);
    Result<IEnumerable<ISchoolReadDto>> GetAll(Func<School, bool>? filter = null, int pageNumber = 1, int pageSize = 20);
}