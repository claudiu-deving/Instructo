using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instructo.Infrastructure.Data.Repositories.Commands;

public class SchoolCommandRepository(AppDbContext appDbContext, ILogger<SchoolCommandRepository> logger) : ICommandRepository<School, Domain.ValueObjects.SchoolId>
{
    public async Task<Result<School>> AddAsync(School entity)
    {
        try
        {
            await appDbContext.Schools.AddAsync(entity);
            var affectedRows = await appDbContext.SaveChangesAsync();

            if(affectedRows==0)
            {
                logger.LogError("Failed to add {school}", entity.Name);
                return Result<School>.Failure([new Error("School-Add", "Failed to add school")]);
            }
            return Result<School>.Success(entity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure([new Error("School-Add-Update-Concurrency", ex.Message)]);
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure([new Error("School-Add", ex.Message)]);
        }
    }

    public async Task<Result<School>> DeleteAsync(School entity)
    {
        try
        {
            appDbContext.Schools.Remove(entity);
            await appDbContext.SaveChangesAsync();
            return Result<School>.Success(entity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure([new Error("School-Delete-Update-Concurrency", ex.Message)]);
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure([new Error("School-Delete", ex.Message)]);
        }
    }

    public async Task<Result<School>> UpdateAsync(School entity)
    {
        try
        {
            appDbContext.Schools.Update(entity);
            await appDbContext.SaveChangesAsync();
            var updatedEntity = await appDbContext.Schools.FindAsync(entity.Id);
            if(updatedEntity==null)
            {
                logger.LogError("Failed to update {school}", new { school = entity.Name });
                return Result<School>.Failure([new Error("School-Update", "Failed to update school")]);
            }
            return Result<School>.Success(updatedEntity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to update {school}", new { school = entity.Name });
            return Result<School>.Failure([new Error("School-Update-Update-Concurrency", ex.Message)]);
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update {school}", new { school = entity.Name });
            return Result<School>.Failure([new Error("School-Update", ex.Message)]);
        }
    }
}
