using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Domain.Entities.SchoolEntities;
using Domain.Models;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;
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
    private readonly List<Address> _locations = [];
    private readonly List<SchoolCategoryPricing> _schoolCategoryPricings = [];

    private School(
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
        Statistics statistics)
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
        Slug=ValueObjects.Slug.Create(companyName);
        City=city;
        County=city.County;
        Slogan=slogan.Value;
        Description=description.Value;
        SchoolStatistics=statistics;
    }

    private School()
    {
    }

    [Timestamp] // EF Core concurrency token
    public byte[] RowVersion { get; private set; } = [];

    [ForeignKey("Owner")]
    public Guid OwnerId { get; private set; }

    public ApplicationUser Owner { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public virtual County? County { get; private set; }
    public virtual City? City { get; private set; }
    public string Slogan { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Statistics SchoolStatistics { get; private set; } = Statistics.Empty;
    public PhoneNumber PhoneNumber { get; private set; } = PhoneNumber.Empty;
    public List<PhoneNumbersGroup> PhoneNumbersGroups { get; private set; } = [];
    public BussinessHours BussinessHours { get; private set; } = BussinessHours.Empty;
    public virtual ICollection<VehicleCategory> VehicleCategories { get; } = [];
    public virtual ICollection<ArrCertificate> Certificates { get; } = [];
    public virtual IReadOnlyCollection<SchoolCategoryPricing> CategoryPricings => _schoolCategoryPricings.AsReadOnly();
    public virtual Image? Icon { get; private set; }
    public virtual IReadOnlyCollection<WebsiteLink> WebsiteLinks => _websiteLinks.AsReadOnly();
    public virtual Team? Team { get; private set; }
    public virtual IReadOnlyCollection<Address> Locations => _locations.AsReadOnly();
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

    public Team CreateTeam()
    {
        if(Team!=null)
            return Team;
        Team=Team.Create(Id);
        return Team;
    }

    public void RemoveTeam()
    {
        Team=null;
    }

    public Result<School> AddExtraLocation(Address address)
    {
        try
        {
            if(_locations.Contains(address))
                return Result<School>.Success(this);
            address.SetSchool(this);
            _locations.Add(address);
            return Result<School>.Success(this);
        }
        catch(Exception ex)
        {
            return Result<School>.Failure([new Error("AddExtraLocation", ex.Message)]);
        }
    }

    public bool RemoveExtraLocation(Address address)
    {
        if(!_locations.Contains(address))
            return false;
        _locations.Remove(address);
        return true;
    }

    public void ChangeName(SchoolName newSchoolName)
    {
        Name=newSchoolName;
    }

    public void SetCategoryPricings(List<SchoolCategoryPricing> categoryPricings)
    {
        _schoolCategoryPricings.Clear();
        foreach(var pricing in categoryPricings)
        {
            if(!_schoolCategoryPricings.Contains(pricing))
                _schoolCategoryPricings.Add(pricing);
        }
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
        City=newData.City;
        County=newData.County;
        Slogan=newData.Slogan;
        Description=newData.Description;
        SchoolStatistics=newData.SchoolStatistics;
        if(newData.Team!=null)
        {
            if(Team==null)
                CreateTeam();
            Team=newData.Team;
        }
        else
        {
            RemoveTeam();
        }
        foreach(var extraLocation in newData.Locations.Where(extraLocation => !_locations.Contains(extraLocation)))
            AddExtraLocation(extraLocation);
    }

    public override bool Equals(object? obj)
    {
        if(obj is not School otherSchool)
            return false;
        return otherSchool.CompanyName.Equals(CompanyName);
    }

    public override int GetHashCode()
    {
        return CompanyName.GetHashCode()*13;
    }

    public override string ToString()
    {
        return CompanyName;
    }

    public static School Create(
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
        Statistics statistics)
    {
        return new School(
            owner,
            name,
            companyName,
            email,
            phoneNumber,
            phoneNumberGroups,
            bussinessHours,
            vehicleCategories,
            certificates,
            icon,
            city,
            slogan,
            description,
            statistics);
    }

    public void ChangeSlogan(Slogan slogan)
    {
        Slogan=slogan.Value;
    }

    public void UpdateName(SchoolName name)
    {
        Name=name.Value;
    }

    public void UpdateEmail(Email email)
    {
        if(!Email.Equals(email.Value))
            Email=email.Value;
    }

    public void UpdateLegalName(LegalName legalName)
    {
        if(!CompanyName.Equals(legalName.Value))
            CompanyName=legalName.Value;
    }

    public void UpdateSlogan(Slogan slogan)
    {
        if(!Slogan.Equals(slogan.Value))
            Slogan=slogan.Value;
    }

    public void UpdateDescription(Description description)
    {
        if(!Description.Equals(description.Value))
            Description=description.Value;
    }

    public void UpdateCity(City city)
    {
        if(City==null||!City.Equals(city))
        {
            City=city;
            County=city.County; // Update county based on the new city
        }
    }

    public void UpdatePhoneNumber(PhoneNumber phoneNumber)
    {
        if(!PhoneNumber.Equals(phoneNumber))
            PhoneNumber=phoneNumber;
    }

    public void UpdateMainLocation(Address mainLocation)
    {
        if(Locations.Count==0||!Locations.First().Equals(mainLocation))
        {
            if(Locations.Count>0)
            {
                var existingMainLocation = Locations.FirstOrDefault(x => x.AddressType==Enums.AddressType.MainLocation);
                if(existingMainLocation!=null)
                    // Update existing main location
                    existingMainLocation.Update(mainLocation.Street, mainLocation.Coordinate?.X.ToString(), mainLocation.Coordinate?.Y.ToString(), Enums.AddressType.MainLocation, mainLocation.Comment);
                else
                {
                    // Add as a new main location if it doesn't exist
                    _locations.Add(mainLocation);
                }
            }
            else
            {
                _locations.Add(mainLocation); // Add if no locations exist
            }
        }
    }

    public void UpdateVehicleCategories(List<VehicleCategory> categories)
    {
        VehicleCategories.Clear();
        foreach(var category in categories)
        {
            if(!VehicleCategories.Contains(category))
                VehicleCategories.Add(category);
        }
    }
}