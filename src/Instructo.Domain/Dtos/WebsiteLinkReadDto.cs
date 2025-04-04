namespace Instructo.Domain.Dtos;

public readonly record struct WebsiteLinkReadDto(string Url, string Name, string Description, ImageReadDto Image);