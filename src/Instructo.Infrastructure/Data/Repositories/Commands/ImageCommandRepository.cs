using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instructo.Infrastructure.Data.Repositories.Commands;

public class ImageCommandRepository(AppDbContext appDbContext, ILogger<SchoolCommandRepository> logger) : ICommandRepository<Domain.Entities.Image, Domain.ValueObjects.ImageId>
{
    public async Task<Result<Image>> AddAsync(Image entity)
    {
        try
        {
            await appDbContext.Images.AddAsync(entity);
            await appDbContext.SaveChangesAsync();
            var addedEntity = await appDbContext.Images.FindAsync(entity.Id);
            if(addedEntity==null)
            {
                logger.LogError("Failed to add {image}", entity.FileName);
                return Result<Image>.Failure([new Error("Image-Add", "Failed to add school")]);
            }
            return Result<Image>.Success(addedEntity);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to add {image}", entity.FileName);
            return Result<Image>.Failure([new Error("Image-Add-Update-Concurrency", ex.Message)]);
        }
        catch(DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add {image}", entity.FileName);
            return Result<Image>.Failure([new Error("Image-Add", ex.Message)]);
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
