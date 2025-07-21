using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;

namespace Domain.Mappers;

public static class WebsiteLinkMappers
{
    public static WebsiteLinkRead ToReadDto(this WebsiteLink link)
    {
        return new WebsiteLinkRead(
            link.Url,
           link.Name,
          link.Description??"",
          link.Icon?.FileName??string.Empty,
          link.Icon?.Url??string.Empty,
          link.Icon?.ContentType??string.Empty,
            link.Icon?.Description??string.Empty
        );
    }
}
