
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;

namespace Infrastructure.Data.Repositories.Queries;
public interface ISchoolCategoryPricingQueryRepository : IQueryRepository<SchoolCategoryPricing, int>
{
    Task<Result<IEnumerable<SchoolCategoryPricing>>> GetByCategoryAsync(VehicleCategoryType categoryId);
    Task<Result<IEnumerable<SchoolCategoryPricing>>> GetBySchoolAsync(Guid schoolId);
}