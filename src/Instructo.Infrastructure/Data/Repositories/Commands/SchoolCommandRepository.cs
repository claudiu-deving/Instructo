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
            var affectedRows = await appDbContext.SaveChangesAsync();

            if (affectedRows == 0)
            {
                logger.LogError("Failed to add {school}", entity.Name);
                return Result<School>.Failure(new Error("School-Add", "Failed to add school"));
            }

            return Result<School>.Success(entity);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure(new Error("School-Add-Update-Concurrency", ex.Message));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add {school}", entity.Name);
            return Result<School>.Failure(new Error("School-Add", ex.Message));
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
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Delete-Update-Concurrency", ex.Message));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to delete {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Delete", ex.Message));
        }
    }

    public async Task<Result<School>> UpdateAsync(School entity)
    {
        try
        {
            //appDbContext.Schools.Update(entity);
            var updatedEntity = await appDbContext.Schools.FindAsync(entity.Id);
            if (updatedEntity == null)
            {
                logger.LogError("Failed to update {school}", new { school = entity.Name });
                return Result<School>.Failure(new Error("School-Update", "Failed to update school"));
            }

            updatedEntity.Update(entity);

            await appDbContext.SaveChangesAsync();
            // if (updatedEntity == null)
            // {
            //    logger.LogError("Failed to update {school}", new { school = entity.Name });
            //     return Result<School>.Failure(new Error("School-Update", "Failed to update school"));
            //  }

            return Result<School>.Success(entity);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to update {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Update-Update-Concurrency", ex.Message));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update {school}", new { school = entity.Name });
            return Result<School>.Failure(new Error("School-Update", ex.Message));
        }
    }

    public async Task<Result<School>> SetApprovalState(SchoolId id, bool isApproved)
    {
        var school = await appDbContext.Schools.FindAsync(id);
        if (school == null)
            return
                Result<School>.Failure(new Error("School-SetApprovalState-NotFound", "School not found"));

        school.IsApproved = isApproved;
        try
        {
            await appDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            try
            {
                foreach (var entry in ex.Entries)
                    if (entry.Entity is School)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues?[property];

                            // TODO: decide which value should be written to database
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(proposedValues);
                    }
                    else
                    {
                        return Result<School>.Failure(new Error("School-SetApprovalState-Concurrency",
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name));
                    }
            }
            catch (Exception ex2)
            {
                logger.LogError(ex2, "Failed to set approval state for {school}", school.Name);
                return Result<School>.Failure(new Error("School-SetApprovalState-Concurrency",
                    "Failed to set approval state for " + school.Name));
            }
        }

        return Result<School>.Success(school);
    }
}