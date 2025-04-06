using Instructo.Domain.Dtos.Image;
using Instructo.Domain.Dtos.Link;
using Instructo.Domain.Entities;

namespace Instructo.Domain.Mappers;

public static class WebsiteLinkMappers
{
    public static WebsiteLinkReadDto ToReadDto(this WebsiteLink link)
    {
        return new WebsiteLinkReadDto
        {
            Url=link.Url,
            Name=link.Name,
            Description=link.Description??"",
            IconData=link.Icon?.ToReadDto()??ImageReadDto.Empty
        };
    }
}
