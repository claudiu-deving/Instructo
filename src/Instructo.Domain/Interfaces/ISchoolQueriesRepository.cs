
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
}