using System.Data.Common;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;
public class AddressQueryRepository(AppDbContext appDbContext) : IQueryRepository<Address, int>
{
    public async Task<Result<IEnumerable<Address>>> GetAllAsync()
    {
        try
        {
            return await appDbContext.Addresses.ToListAsync();
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAllAddresses-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAllAddresses-Db", ex.Message);
        }
    }

    public async Task<Result<Address?>> GetByIdAsync(int id)
    {
        try
        {
            return await appDbContext.Addresses.FirstOrDefaultAsync(x => x.Id==id);
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetAddressById-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAddressById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAddressById-Db", ex.Message);
        }
    }

    public async Task<Result<Address?>> GetByIndexed(string indexedValue)
    {
        try
        {
            return await appDbContext.Addresses.Include(x => x.School).FirstOrDefaultAsync(x => x.School.Id.ToString()==indexedValue);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetAddressByName-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetAddressByName-Db", ex.Message);
        }
    }
}
