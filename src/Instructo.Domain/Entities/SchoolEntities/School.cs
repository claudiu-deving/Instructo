using System.ComponentModel.DataAnnotations.Schema;

using Domain.ValueObjects;

namespace Domain.Entities.SchoolEntities;
/// <summary>
/// The school
/// </summary>
/// <remarks>
/// One School Entity can have only one Owner and vice-versa.
///</remarks>
public class School : BaseAuditableEntity<SchoolId>
{
    public School(
        ApplicationUser owner,
        SchoolName name,
        LegalName companyName,
        Email email,
        PhoneNumber phoneNumber,
        List<PhoneNumbersGroup> phoneNumberGroups,
        BussinessHours bussinessHours,
         List<VehicleCategory> vehicleCategories,
          List<ArrCertificate> certificates,
        Image? icon) : base()
    {
        Id=SchoolId.CreateNew();
        Owner=owner;
        Name=name;
        CompanyName=companyName;
        Icon=icon;
        Email=email;
        PhoneNumber=phoneNumber;
        PhoneNumbersGroups=phoneNumberGroups;
        BussinessHours=bussinessHours;
        VehicleCategories=vehicleCategories;
        Certificates=certificates;
    }

    private School() { }

    [ForeignKey("Owner")]
    public Guid OwnerId { get; private set; }
    public ApplicationUser Owner { get; private set; } = null!;
    public SchoolName Name { get; private set; }
    public LegalName CompanyName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = PhoneNumber.Empty;
    public List<PhoneNumbersGroup> PhoneNumbersGroups { get; private set; } = [];
    public BussinessHours BussinessHours { get; private set; } = BussinessHours.Empty;
    public virtual List<VehicleCategory> VehicleCategories { get; private set; } = [];
    public virtual List<ArrCertificate> Certificates { get; private set; } = [];
    public virtual Image? Icon { get; private set; }

    private readonly List<WebsiteLink> _websiteLinks = [];
    public virtual IReadOnlyCollection<WebsiteLink> WebsiteLinks => _websiteLinks.AsReadOnly();

    public void AddLink(WebsiteLink link) =>
        _websiteLinks.Add(link);

    public bool RemoveLink(WebsiteLink link) =>
        _websiteLinks.Remove(link);
}
