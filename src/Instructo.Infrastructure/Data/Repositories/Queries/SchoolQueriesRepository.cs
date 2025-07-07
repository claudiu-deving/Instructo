using System.Data.Common;

using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class SchoolQueriesRepository : IQueryRepository<School, SchoolId>, ISchoolQueriesRepository
{
    private readonly AppDbContext _dbContext;

    public SchoolQueriesRepository(AppDbContext dbContext)
    {
        _dbContext=dbContext;
    }

    public async Task<Result<IEnumerable<School>?>> GetAllAsync()
    {
        try
        {
            return Result<IEnumerable<School>?>.Success(
                await _dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.Certificates)
                    .Include(x => x.County)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Take(10)
                    .ToListAsync());
        }
        catch(OperationAbortedException ex)
        {
            return Result<IEnumerable<School>?>.Failure(new Error("GetAllSchools-Aborted", ex.Message));
        }
        catch(DbException ex)
        {
            return Result<IEnumerable<School>?>.Failure(new Error("GetAllSchools-Db", ex.Message));
        }
    }

    public async Task<Result<School?>> GetBySlugAsync(string slug)
    {
        try
        {
            return await
                _dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.Certificates)
                    .Include(x => x.City)
                    .ThenInclude(city => city.County)
                    .Include(x=>x.Address)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.Slug==slug);
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolBySlug-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolBySlug-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolBySlug-Db", ex.Message);
        }
    }


    public async Task<Result<School?>> GetByIdAsync(SchoolId id)
    {
        try
        {
            return Result<School?>.Success(await
                _dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.Certificates)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.Id==id));
        }
        catch(InvalidOperationException ex)
        {
            return Result<School?>.Failure(new Error("GetSchoolById-Empty", ex.Message));
        }
        catch(OperationAbortedException ex)
        {
            return Result<School?>.Failure(new Error("GetSchoolById-Aborted", ex.Message));
        }
        catch(DbException ex)
        {
            return Result<School?>.Failure(new Error("GetSchoolById-Db", ex.Message));
        }
    }

    public async Task<Result<School?>> GetByIndexed(string companyName)
    {
        return Result<School?>.Success(await
            _dbContext.Schools
                .Include(x => x.Owner)
                .Include(x => x.WebsiteLinks)
                .Include(x => x.VehicleCategories)
                .Include(x => x.Certificates)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.CompanyName==companyName));
    }
}