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
    public School(ApplicationUser owner, SchoolName name, CompanyName companyName, Image? icon) : base()
    {
        Id=SchoolId.CreateNew();
        Owner=owner;
        Name=name;
        CompanyName=companyName;
        Icon=icon;
    }

    private School() { }

    public virtual ApplicationUser? Owner { get; private set; }
    public SchoolName Name { get; private set; }
    public CompanyName CompanyName { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }

    public virtual Image? Icon { get; private set; }

    private readonly List<WebsiteLink> _websiteLinks = [];
    public virtual IReadOnlyCollection<WebsiteLink> WebsiteLinks => _websiteLinks.AsReadOnly();

    public void AddLink(WebsiteLink link) =>
        _websiteLinks.Add(link);

    public bool RemoveLink(WebsiteLink link) =>
        _websiteLinks.Remove(link);
}
