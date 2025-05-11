using System.Data.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class VehicleCategoryQueriesRepository(AppDbContext dbContext)
    : IQueryRepository<VehicleCategory, VehicleCategoryType>
{
    public async Task<Result<IEnumerable<VehicleCategory>>> GetAllAsync()
    {
        try
        {
            return Result<IEnumerable<VehicleCategory>>.Success(await dbContext.Categories.ToListAsync());
        }
        catch (OperationAbortedException ex)
        {
            return Result<IEnumerable<VehicleCategory>>.Failure(new Error("GetAllVehicleCategories-Aborted",
                ex.Message));
        }
        catch (DbException ex)
        {
            return Result<IEnumerable<VehicleCategory>>.Failure(new Error("GetAllVehicleCategories-Db", ex.Message));
        }
    }

    public async Task<Result<VehicleCategory?>> GetByIdAsync(VehicleCategoryType id)
    {
        try
        {
            return Result<VehicleCategory?>.Success(await dbContext.Categories.FindAsync(id));
        }
        catch (InvalidOperationException ex)
        {
            return Result<VehicleCategory?>.Failure(new Error("GetVehicleCategoryById-Empty", ex.Message));
        }
        catch (OperationAbortedException ex)
        {
            return Result<VehicleCategory?>.Failure(new Error("GetVehicleCategoryById-Aborted", ex.Message));
        }
        catch (DbException ex)
        {
            return Result<VehicleCategory?>.Failure(new Error("GetVehicleCategoryById-Db", ex.Message));
        }
    }

    public Task<Result<VehicleCategory>?> GetByIndexed(string companyName)
    {
        throw new NotImplementedException();
    }
}