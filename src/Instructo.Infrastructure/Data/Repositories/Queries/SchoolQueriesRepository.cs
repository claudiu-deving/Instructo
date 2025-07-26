using System.Data.Common;

using Domain.Dtos.School;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Repositories.Queries;

public class SchoolQueriesRepository(AppDbContext dbContext, ILogger<SchoolQueriesRepository> logger) : ISchoolQueriesRepository
{
    public async Task<Result<SchoolDetailReadDto?>> GetBySlugAsync(string slug)
    {
        try
        {
            return await dbContext.SchoolDetails.AsNoTracking().FirstOrDefaultAsync(x => x.Slug==slug);
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
            return await dbContext.Schools
                    .Include(x => x.Owner)
                    .Include(x => x.WebsiteLinks)
                    .Include(x => x.Certificates)
                    .Include(x => x.VehicleCategories)
                    .Include(x => x.City)
                    .Include(x => x.County)
                    .Include(x => x.Locations)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.Id==id.Id);
        }
        catch(InvalidOperationException ex)
        {
            return new Error("GetSchoolById-Empty", ex.Message);
        }
        catch(OperationAbortedException ex)
        {
            return new Error("GetSchoolById-Aborted", ex.Message);
        }
        catch(DbException ex)
        {
            return new Error("GetSchoolById-Db", ex.Message);
        }
    }

    public async Task<Result<bool>> SchoolExists(string companyName)
    {
        return Result<bool>.Success(await
            dbContext.Schools
                .FirstOrDefaultAsync(x => x.CompanyName==companyName) is not null);
    }
    public Result<IEnumerable<ISchoolReadDto>> GetAll(
        Func<School, bool>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        List<Error> errors = [];
        var result = dbContext.Schools
                .Include(school => school.Owner)
              .Include(school => school.County)
              .Include(school => school.City)
              .Include(school => school.Icon)
              .Include(school => school.VehicleCategories)
              .AsNoTracking()
              .AsSplitQuery()
             .Where(filter??(x => true))
              .Skip((pageNumber-1)*pageSize)
              .Take(pageSize)
              .Select(x => (ISchoolReadDto)Map(x)!);


        if(errors.Count>0)
        {
            return errors.ToArray();
        }
        else
        {
            return Result<IEnumerable<ISchoolReadDto>>.Success(result);
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
              .AsNoTracking()
              .AsSplitQuery()
              .ToListAsync())
               .Select(x => (SchoolReadDto)Map(x)!));
    }
    private SchoolReadDto? Map(School? school)
    {
        try
        {
            if(school is null)
            {
                return null;
            }
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