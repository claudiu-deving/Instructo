using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;


namespace Instructo.Infrastructure.Data.Repositories.Commands
{
    public class ImageCommandRepository(AppDbContext appDbContext) : ICommandRepository<Image, ImageId>
    {
        public async Task<Result<string>> AddAsync(Image entity)
        {
            try
            {
                await appDbContext.Images.AddAsync(entity);
                var affectedRows = await appDbContext.SaveChangesAsync();

                if(affectedRows>0)
                {
                    return Result<string>.Success(entity.Url.ToString());
                }
                else
                {
                    return Result<string>.Failure([new Error("Image-Add", "Error adding image")]);
                }
            }
            catch(DbUpdateException ex)
            {
                return Result<string>.Failure([new Error("Image-Add-Db_Update", ex.Message)]);
            }
        }
        public async Task<Result<string>> DeleteAsync(Image entity)
        {
            try
            {
                appDbContext.Images.Remove(entity);
                var affectedRows = await appDbContext.SaveChangesAsync();
                if(affectedRows>0)
                {
                    return Result<string>.Success(entity.Url.ToString());
                }
                else
                {
                    return Result<string>.Failure([new Error("Image-Delete", "Error deleting image,no rows affected")]);
                }
            }
            catch(DbUpdateException ex)
            {
                return Result<string>.Failure([new Error("Image-Delete-Db_Update", ex.Message)]);
            }
        }
        public async Task<Result<string>> UpdateAsync(Image entity)
        {
            try
            {
                appDbContext.Images.Update(entity);
                var affectedRows = await appDbContext.SaveChangesAsync();
                if(affectedRows>0)
                {
                    return Result<string>.Success(entity.Url.ToString());
                }
                else
                {
                    return Result<string>.Failure([new Error("Image-Update", "Error updating image")]);
                }
            }
            catch(DbUpdateException ex)
            {
                return Result<string>.Failure([new Error("Image-Update-Db_Update", ex.Message)]);
            }
        }
    }
}
