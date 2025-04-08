using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Entities;

namespace Domain.Mappers;

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
