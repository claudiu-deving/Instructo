using System.Data.Common;

using Domain.Entities;
using Domain.Enums;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;
public class SchoolCategoryPricingQueryRepository(AppDbContext appDbContext) : ISchoolCategoryPricingQueryRepository
{
    public async Task<Result<IEnumerable<SchoolCategoryPricing>>> GetAllAsync()
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings.ToListAsync();
            return result;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAllSchoolCategoryPricings-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAllSchoolCategoryPricings-Db", ex.Message);
        }
    }

    public async Task<Result<SchoolCategoryPricing?>> GetByIdAsync(int id)
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings.FindAsync(id);
            return result;
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolCategoryPricingById-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolCategoryPricingById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolCategoryPricingById-Db", ex.Message);
        }
    }

    public async Task<Result<SchoolCategoryPricing?>> GetByIndexed(string indexedValue)
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings.FirstOrDefaultAsync(s => s.FullPrice.ToString()==indexedValue);
            return result;
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolCategoryPricingByIndexed-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolCategoryPricingByIndexed-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolCategoryPricingByIndexed-Db", ex.Message);
        }
    }

    public async Task<Result<IEnumerable<SchoolCategoryPricing>>> GetByCategoryAsync(VehicleCategoryType categoryId)
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings
                .Include(s => s.Category)
                .Where(s => s.Category.Id==(int)categoryId)
                .ToListAsync();
            return result;
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolCategoryPricingByCategory-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolCategoryPricingByCategory-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolCategoryPricingByCategory-Db", ex.Message);
        }
    }

    public async Task<Result<IEnumerable<SchoolCategoryPricing>>> GetBySchoolAsync(Guid schoolId)
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings
                .Include(s => s.School)
                .Where(s => s.School.Id==schoolId)
                .ToListAsync();
            return result;
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchool-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchool-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchool-Db", ex.Message);
        }
    }

    public async Task<Result<SchoolCategoryPricing?>> GetBySchoolAndCategory(Guid schoolId, VehicleCategoryType categoryId)
    {
        try
        {
            var result = await appDbContext.SchoolCategoryPricings
                .Include(s => s.School)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.School.Id==schoolId&&s.Category.Id==(int)categoryId);
            return result;
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchoolAndCategory-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchoolAndCategory-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolCategoryPricingBySchoolAndCategory-Db", ex.Message);
        }
    }
}
