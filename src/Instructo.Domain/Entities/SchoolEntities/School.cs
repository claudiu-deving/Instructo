using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities.SchoolEntities;
[DebuggerDisplay("School: {Name.Value}, Company: {CompanyName.Value}, Owner: {Owner.FirstName} {Owner.LastName}, Email: {Email.Value}")]
/// <summary>
/// The school
/// </summary>
/// <remarks>
/// One School Entity can have only one Owner and vice-versa.
/// </remarks>
public class School : BaseAuditableEntity<Guid>
{
    private readonly List<WebsiteLink> _websiteLinks = [];

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
        Image? icon,
        City city,
        Slogan slogan,
        Description description,
        Address address)
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
        Slug=Slug.Create(CompanyName);
        City=city;
        County=city.County;
        Slogan=slogan;
        Description=description;
        Address=address;
    }

    private School()
    {
        Address=Address.Empty;
    }

    [Timestamp] // EF Core concurrency token
    public byte[] RowVersion { get; private set; } = [];

    [ForeignKey("Owner")]
    public Guid OwnerId { get; private set; }

    public ApplicationUser Owner { get; private set; } = null!;
    public SchoolName Name { get; private set; }
    public LegalName CompanyName { get; private set; }
    public Email Email { get; private set; }
    public Slug Slug { get; }
    public virtual County? County { get; init; }
    public virtual City? City { get; private set; }
    public virtual Address Address { get; private set; }
    public Slogan Slogan { get; }
    public Description Description { get; }
    public PhoneNumber PhoneNumber { get; private set; } = PhoneNumber.Empty;
    public List<PhoneNumbersGroup> PhoneNumbersGroups { get; private set; } = [];
    public BussinessHours BussinessHours { get; private set; } = BussinessHours.Empty;
    public virtual ICollection<VehicleCategory> VehicleCategories { get; } = [];
    public virtual ICollection<ArrCertificate> Certificates { get; } = [];
    public virtual Image? Icon { get; private set; }
    public virtual IReadOnlyCollection<WebsiteLink> WebsiteLinks => _websiteLinks.AsReadOnly();

    public bool IsApproved { get; set; }

    public void Approve()
    {
        IsApproved=true;
    }

    public void Reject()
    {
        IsApproved=false;
    }

    public Result<School> AddLink(WebsiteLink link)
    {
        _websiteLinks.Add(link);
        return this;
    }

    public bool RemoveLink(WebsiteLink link)
    {
        return _websiteLinks.Remove(link);
    }

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

    public void Update(School newData)
    {
        Name=newData.Name;
        CompanyName=newData.CompanyName;
        Email=newData.Email;
        PhoneNumber=newData.PhoneNumber;
        PhoneNumbersGroups=newData.PhoneNumbersGroups;
        BussinessHours=newData.BussinessHours;
        Icon=newData.Icon;
        foreach(var link in newData.WebsiteLinks)
            if(!WebsiteLinks.Contains(link))
                AddLink(link);
        foreach(var link in newData.Certificates.Where(link => !Certificates.Contains(link)))
            AddCertificate(link);

        foreach(var vehicleCategory in newData.VehicleCategories.Where(vehicleCategory =>
                     !VehicleCategories.Contains(vehicleCategory)))
            AddVehicleCategory(vehicleCategory);
    }

    public override bool Equals(object? obj)
    {
        if(obj is not School otherSchool)
        {
            return false;
        }
        return otherSchool.CompanyName.Value.Equals(this.CompanyName.Value);
    }

    public override int GetHashCode()
    {
        return CompanyName.Value.GetHashCode()*13;
    }

    public override string ToString()
    {
        return CompanyName.Value;
    }
}