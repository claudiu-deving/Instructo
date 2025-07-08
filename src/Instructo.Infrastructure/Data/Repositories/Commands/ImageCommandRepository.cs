using System.Data;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Repositories.Commands;

public class ImageCommandRepository(AppDbContext appDbContext, ILogger<ImageCommandRepository> logger)
    : ICommandRepository<Image, ImageId>
{
    public async Task<Result<Image>> AddAsync(Image entity)
    {
        try
        {
            await appDbContext.Images.AddAsync(entity);
            var addedEntity = await appDbContext.Images.FindAsync(entity.Id);
            if(addedEntity==null)
            {
                logger.LogError("Failed to add {image}", entity.FileName);
                return Result<Image>.Failure(new Error("Image-Add", "Failed to add school"));
            }

            return Result<Image>.Success(addedEntity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to add {image}", entity.FileName);
            return Result<Image>.Failure(new Error("Image-Add-Update-Concurrency", ex.Message));
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add {image}", entity.FileName);
            return Result<Image>.Failure(new Error("Image-Add", ex.Message));
        }
    }

    public Task<Result<Image>> DeleteAsync(Image entity)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Image>> UpdateAsync(Image entity)
    {
        throw new NotImplementedException();
    }
}