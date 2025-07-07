using System.Data.Common;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class CityQueryRepository(AppDbContext appDbContext) : IQueryRepository<City, int>
{
    public async Task<Result<IEnumerable<City>>> GetAllAsync()
    {
        try
        {
            var cities = await appDbContext.Cities.Include(x => x.County).ToListAsync();
            return cities;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAllCities-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAllCities-Db", ex.Message);
        }
    }

    public async Task<Result<City?>> GetByIdAsync(int id)
    {
        try
        {
            var city = await appDbContext.Cities.Include(x => x.County).FirstOrDefaultAsync(x => x.Id==id);
            return city;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetCity-ById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetCity-ById-Db", ex.Message);
        }
    }

    public async Task<Result<City?>> GetByIndexed(string indexedValue)
    {
        try
        {
            var city = await appDbContext.Cities.Include(x => x.County).FirstOrDefaultAsync(x => x.Name==indexedValue);
            return city;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetCity-ByName-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetCity-ByName-Db", ex.Message);
        }
    }
}
