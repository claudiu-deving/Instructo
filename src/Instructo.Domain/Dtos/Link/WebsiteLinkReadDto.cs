using Domain.Dtos.Image;

namespace Domain.Dtos.Link;

public readonly record struct WebsiteLinkReadDto(string Url, string Name, string? Description, ImageReadDto IconData)
{
    public static WebsiteLinkReadDto Empty => new(string.Empty, string.Empty, string.Empty, new ImageReadDto(string.Empty, string.Empty, string.Empty, string.Empty));
}
public readonly record struct WebsiteLinkUpdateDto(string? Url, string? Name, string? Description, ImageUpdateDto? IconData)
{
    public static WebsiteLinkUpdateDto Empty => new(string.Empty, string.Empty, string.Empty, new ImageUpdateDto(string.Empty, string.Empty, string.Empty, string.Empty));
}

public readonly record struct SocialMediaLinkDto(string Url, string SocialPlatformName)
{
    public static SocialMediaLinkDto Empty => new(string.Empty, string.Empty);
}