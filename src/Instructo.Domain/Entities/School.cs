using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities;
/// <summary>
/// The school
/// </summary>
/// <remarks>
/// One School can have only one Owner and vice-versa.
///</remarks>
public class School : BaseAuditableEntity<SchoolId>
{
    internal School(ApplicationUser owner, SchoolName name, CompanyName companyName, Image? icon) : base()
    {
        Owner=owner;
        Name=name;
        CompanyName=companyName;
        Icon=icon;
    }

    private School() { }

    public virtual ApplicationUser? Owner { get; private set; }
    public SchoolName Name { get; private set; }
    public CompanyName CompanyName { get; private set; }
    public virtual Image? Icon { get; private set; }

    private readonly List<WebsiteLink> _websiteLinks = [];
    public virtual IReadOnlyCollection<WebsiteLink> WebsiteLinks => _websiteLinks.AsReadOnly();

    public void AddLink(WebsiteLink link) =>
        _websiteLinks.Add(link);

    public bool RemoveLink(WebsiteLink link) =>
        _websiteLinks.Remove(link);
}



public class WebsiteLink : BaseAuditableEntity<WebsiteLinkId>
{
    public WebsiteLink(string url, string name, string description, Image icon)
    {
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
}

public sealed class Image : BaseAuditableEntity<ImageId>
{
    private Image() { }
    public FileName FileName { get; private set; }
    public ContentType ContentType { get; private set; }
    public Url Url { get; private set; }
    public string? Description { get; private set; }

    public Image(string fileName, string contentType, string url, string? description)
    {
        FileName=fileName;
        ContentType=contentType;
        Url=url;
        Description=description;
    }
}
