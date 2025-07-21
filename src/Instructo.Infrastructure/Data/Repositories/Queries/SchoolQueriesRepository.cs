using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Domain.Dtos;
using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Repositories.Queries;

public class SchoolQueriesRepository(AppDbContext dbContext, ILogger<SchoolQueriesRepository> logger) : ISchoolQueriesRepository
{

    public async Task<Result<IEnumerable<SchoolDetailReadDto>>> GetAllDetailedAsync()
    {
        return await dbContext.SchoolDetails.AsNoTracking().ToListAsync();
    }

    public async Task<Result<School?>> GetBySlugAsync(string slug)
    {
        try
        {
            return await
                dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.Certificates)
                    .Include(x => x.City)
                    .ThenInclude(city => city.County)
                    .Include(x => x.Address)
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
                dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.Certificates)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.Id==id.Id));
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
            dbContext.Schools
                .Include(x => x.Owner)
                .Include(x => x.WebsiteLinks)
                .Include(x => x.VehicleCategories)
                .Include(x => x.Certificates)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.CompanyName==companyName));
    }
    public Result<IEnumerable<ISchoolReadDto>> GetAll(
        Func<SchoolDetailReadDto, bool>? filter = null,
        int pageNumber = 1,
        int pageSize = 20,
        bool withDetails = false)
    {
        List<Error> errors = [];
        var result = dbContext.SchoolDetails
             .Where(filter??(x => true))
              .Skip((pageNumber-1)*pageSize)
              .Take(pageSize);

        if(errors.Count>0)
        {
            return errors.ToArray();
        }
        else
        {
            return Result<IEnumerable<ISchoolReadDto>>.Success(
                result.ToList().Where(x => x!=null).Select(x => x!));
        }
    }

    public async Task<Result<IEnumerable<SchoolReadDto>>> GetAllAsync()
    {
        return Result<IEnumerable<SchoolReadDto>>.Success(
              (await dbContext.Schools
              .Include(school => school.Owner)
              .Include(school => school.County)
              .Include(school => school.City)
              .Include(school => school.Icon)
              .Include(school => school.VehicleCategories)
              .Include(school => school.Address)
              .AsNoTracking()
              .AsSplitQuery()
              .ToListAsync())
               .Select(Map));
    }

    private SchoolReadDto Map(School school)
    {
        try
        {
            var schoolCategories = dbContext.SchoolCategories.Include(x => x.VehicleCategory).ToList();
            var arrCertificates = dbContext.SchoolCertificates.Include(x => x.Certificate).ToList();
            return new SchoolReadDto(
                                    school.Id,
                                    school.Name,
                                    school.CompanyName,
                                    school.Email,
                                    school.PhoneNumber.Value,
                                    school.Slug,
                                    school.County!.Code,
                                    school.City!.Name,
                                    school.Slogan,
                                    school.Description,
                                    school.Address.Street,
                                    new Domain.Dtos.Image.ImageReadDto(school.Icon!.FileName, school.Icon.Url, school.Icon.ContentType, school.Icon.Description)
                                    );
        }
        catch(NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
            logger.LogError("Mapping {school} failed with exception {ex}", school, ex);
            throw;
        }
    }
}