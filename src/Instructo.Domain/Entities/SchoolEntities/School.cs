using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Shared;
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
    
    [Timestamp] // EF Core concurrency token
    public byte[] RowVersion { get; private set; }

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

    public Result<School> AddLink(WebsiteLink link)
    {
        _websiteLinks.Add(link);
        return this;
    }

    public bool RemoveLink(WebsiteLink link) =>
        _websiteLinks.Remove(link);

    public void AddVehicleCategory(VehicleCategory vehicleCategory)
    {
        if(VehicleCategories.Contains(vehicleCategory))
            return;
        VehicleCategories.Add(vehicleCategory);
    }

    public bool RemoveVehicleCategory(VehicleCategory vehicleCategory)
    {
        if(!VehicleCategories.Contains(vehicleCategory))
            return false;
        VehicleCategories.Remove(vehicleCategory);
        return true;
    }
    
    public void AddCertificate(ArrCertificate certificate)
    {
        if(Certificates.Contains(certificate))
            return;
        Certificates.Add(certificate);
    }
    public bool RemoveCertificate(ArrCertificate certificate)
    {
        if(!Certificates.Contains(certificate))
            return false;
        Certificates.Remove(certificate);
        return true;
    }

    public void AddLogo(Image schoolLogo)
    {
        Icon=schoolLogo;
    }

    public void ChangeName(SchoolName newSchoolName)
    {
        Name=newSchoolName;
    }
}
