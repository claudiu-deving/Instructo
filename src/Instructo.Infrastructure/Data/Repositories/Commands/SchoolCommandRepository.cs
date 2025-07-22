using System.Data;

using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Repositories.Commands;

public class SchoolCommandRepository(AppDbContext appDbContext, ILogger<SchoolCommandRepository> logger)
    : ISchoolCommandRepository
{
    public async Task<Result<School>> AddAsync(School entity)
    {
        try
        {
            await appDbContext.Schools.AddAsync(entity);

            return Result<School>.Success(entity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure(new Error("School-Add-Update-Concurrency", ex.Message));
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure(new Error("School-Add", ex.Message));
        }
    }

    public async Task<Result<School>> DeleteAsync(School entity)
    {
        try
        {
            var rowsAffected = await appDbContext.Schools
                             .Where(s => s.Id==entity.Id)
                             .ExecuteDeleteAsync();
            if(rowsAffected==0)
            {
                return new Error("School-Delete-No-Rows", $"No rows affected during the removal of: {entity.Slug}");
            }
            return Result<School>.Success(entity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Delete-Update-Concurrency", ex.Message));
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Delete", ex.Message));
        }
    }

    public async Task<Result<School>> UpdateAsync(School entity)
    {
        try
        {
            var updatedEntity = await appDbContext.Schools.FindAsync(entity.Id);
            if(updatedEntity==null)
            {
                logger.LogError("Failed to update {school}", new { school = entity.Name });
                return Result<School>.Failure(new Error("School-Update", "Failed to update school"));
            }

            updatedEntity.Update(entity);
            return Result<School>.Success(updatedEntity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to update {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Update-Update-Concurrency", ex.Message));
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update {school}", entity.Name);
            return Result<School>.Failure(new Error("School-Update", ex.Message));
        }
    }

    public async Task<Result<School>> SetApprovalState(Guid id, bool isApproved)
    {
        try
        {
            var school = await appDbContext.Schools.FindAsync(id);
            if(school==null)
            {
                logger.LogError("School not found for approval state change: {schoolId}", id);
                return Result<School>.Failure(new Error("School-NotFound", "School not found"));
            }

            school.IsApproved=isApproved;
            return Result<School>.Success(school);
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update approval state for school {schoolId}", id);
            return Result<School>.Failure(new Error("School-Approval-Update", ex.Message));
        }
    }
}