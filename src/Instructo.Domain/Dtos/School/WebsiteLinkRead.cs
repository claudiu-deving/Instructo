namespace Domain.Dtos.School;

public class WebsiteLinkRead(
    string url,
    string name,
    string? description,
    string iconFileName,
    string iconUrl,
    string iconContentType,
    string? iconDescription)
{
    public string Url { get; init; } = url;
    public string Name { get; init; } = name;
    public string? Description { get; init; } = description;
    public string IconFileName { get; init; } = iconFileName;
    public string IconUrl { get; init; } = iconUrl;
    public string IconContentType { get; init; } = iconContentType;
    public string? IconDescription { get; init; } = iconDescription;
}