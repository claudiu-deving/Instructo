using System.Data.Common;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class TransmissionQueryRepository(AppDbContext appDbContext) : IQueryRepository<Transmission, int>
{
    public async Task<Result<IEnumerable<Transmission>>> GetAllAsync()
    {
        try
        {
            var cities = await appDbContext.Transmissions.ToListAsync();
            return cities;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAllTransmissions-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAllTransmissions-Db", ex.Message);
        }
    }

    public async Task<Result<Transmission?>> GetByIdAsync(int id)
    {
        try
        {
            var city = await appDbContext.Transmissions.FirstOrDefaultAsync(x => x.Id==id);
            return city;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetTransmission-ById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetTransmission-ById-Db", ex.Message);
        }
    }

    public async Task<Result<Transmission?>> GetByIndexed(string indexedValue)
    {
        try
        {
            var city = await appDbContext.Transmissions.FirstOrDefaultAsync(x => x.Name==indexedValue);
            return city;
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetTransmission-ByName-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetTransmission-ByName-Db", ex.Message);
        }
    }
}
