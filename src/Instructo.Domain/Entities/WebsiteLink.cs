using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Entities.SchoolEntities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class WebsiteLink : BaseAuditableEntity<WebsiteLinkId>
{
    private readonly List<School> _schools = [];
    private WebsiteLink() { }

    public Url Url { get; private set; }
    public WebsiteLinkName Name { get; private set; }
    public string? Description { get; private set; }
    public virtual Image? Icon { get; private set; }
    public virtual IReadOnlyCollection<School> Schools => _schools.AsReadOnly();

    public static Result<WebsiteLink> Create(string url, string name, string description, Image icon)
    {
        var websiteLink = new WebsiteLink();
        var errors = new List<Error>();
        var urlResult = Url.Create(url)
            .OnSuccess(url => websiteLink.Url=url)
            .OnError(errors.AddRange);
        var nameResult = WebsiteLinkName.Create(name)
            .OnSuccess(name => websiteLink.Name=name)
            .OnError(errors.AddRange);
        websiteLink.Description=description;
        websiteLink.Icon=icon;
        websiteLink.Id=WebsiteLinkId.CreateNew();
        if(errors.Count>0)
            return Result<WebsiteLink>.Failure([.. errors]);
        return websiteLink;
    }

    public WebsiteLinkReadDto ToDto()
    {
        return new WebsiteLinkReadDto
        {
            Url=Url,
            Name=Name,
            Description=Description??"",
            IconData=Icon?.ToDto()??new ImageReadDto()
        };
    }
}
