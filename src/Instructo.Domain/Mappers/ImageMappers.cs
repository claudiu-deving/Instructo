using Domain.Dtos.Image;
using Domain.Entities;

namespace Domain.Mappers;

public static class ImageMappers
{
    public static ImageReadDto ToReadDto(this Image image)
    {
        return new ImageReadDto
        {
            FileName=image.FileName.Value,
            Url=image.Url.Value,
            ContentType=image.ContentType.Value,
            Description=image.Description??""
        };
    }
}
