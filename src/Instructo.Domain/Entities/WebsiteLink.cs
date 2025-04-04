using Instructo.Domain.Dtos;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities;

public class WebsiteLink : BaseAuditableEntity<WebsiteLinkId>
{
    private readonly List<School> _schools = [];
    public WebsiteLink(string url, string name, string description, Image icon)
    {
        Id=WebsiteLinkId.CreateNew();
        Url=url;
        Name=name;
        Description=description;
        Icon=icon;
    }

    private WebsiteLink() { }
    public Url Url { get; private set; }
    public WebsiteLinkName Name { get; private set; }
    public string? Description { get; private set; }
    public virtual Image? Icon { get; private set; }
    public virtual IReadOnlyCollection<School> Schools => _schools.AsReadOnly();

    public WebsiteLinkReadDto ToDto()
    {
        return new WebsiteLinkReadDto
        {
            Url=Url,
            Name=Name,
            Description=Description??"",
            Image=Icon?.ToDto()??new ImageReadDto()
        };
    }
}
