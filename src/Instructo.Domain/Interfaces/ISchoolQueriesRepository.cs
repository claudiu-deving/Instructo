
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Interfaces;
public interface ISchoolQueriesRepository
{
    Task<Result<School?>> GetByIdAsync(SchoolId id);
    Task<Result<School?>> GetByIndexed(string indexedValue);
    Task<Result<IEnumerable<SchoolDetailReadDto>>> GetAllDetailedAsync();
    Task<Result<School?>> GetBySlugAsync(string slug);
    Task<Result<IEnumerable<SchoolReadDto>>> GetAllAsync();
    Result<IEnumerable<ISchoolReadDto>> GetAll(Func<School, bool>? filter = null, int pageNumber = 1, int pageSize = 20, bool withDetails = false);
}