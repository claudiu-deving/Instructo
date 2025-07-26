using Domain.Entities;
using Domain.Shared;

namespace Infrastructure.Data.Repositories.Queries;
public class ImagesQueryRepository(AppDbContext appDbContext) : IImagesQueryRepository
{
    public Result<Image> GetImageByUrl(string imageUrl)
    {
        if(string.IsNullOrWhiteSpace(imageUrl))
            return new Error("Image URL cannot be null or empty", nameof(imageUrl));
        var image = appDbContext.Images.FirstOrDefault(img => img.Url==imageUrl);
        if(image==null)
            return new Error("Image not found", nameof(imageUrl));
        return Result<Image>.Success(image);
    }
}
