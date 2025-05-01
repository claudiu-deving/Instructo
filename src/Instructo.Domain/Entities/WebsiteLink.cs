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

    public Result<WebsiteLink> Update(string? url, string? name, string? description, Image? icon)
    {
        var errors = new List<Error>();
        if (url is not null)
        {
            var urlResult = Url.Create(url)
                .OnSuccess(lnk => this.Url = lnk)
                .OnError(errors.AddRange);
        }

        if (name is not null)
        {
            var nameResult = WebsiteLinkName.Create(name)
                .OnSuccess(n => this.Name = n)
                .OnError(errors.AddRange);
        }

        if (description is not null)
        {
            this.Description = description;
        }

        if (icon is not null)
        {
            this.Icon = icon;
        }

        return errors.Count>0 
            ? Result<WebsiteLink>.Failure([.. errors]) 
            : this;
    }

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
        return errors.Count>0 
            ? Result<WebsiteLink>.Failure([.. errors]) 
            : websiteLink;
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
