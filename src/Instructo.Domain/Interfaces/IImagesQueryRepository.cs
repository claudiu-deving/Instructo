using Domain.Entities;
using Domain.Shared;

namespace Infrastructure.Data.Repositories.Queries;
public interface IImagesQueryRepository
{
    Result<Image> GetImageByUrl(string imageUrl);
}