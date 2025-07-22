using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

namespace Domain.Dtos.School;
/// <summary>
/// The data required to show the School on its own dedicated page
/// </summary>
public class SchoolDetailReadDto : ISchoolReadDto
{
    public SchoolDetailReadDto(
        Guid Id,
        string Name,
        string CompanyName,
        string Email,
        string PhoneNumber,
        string Slug,
        string CountyId,
        string CityName,
        string Slogan,
        string Description,
        IEnumerable<PhoneNumberGroupDto> PhoneNumberGroups,
        ImageReadDto IconData,
        WebsiteLinkRead[] Links,
        BussinessHours BussinessHours,
        List<VehicleCategoryDto> VehicleCategories,
        List<ArrCertificationDto> Certificates,
        SchoolStatisticsDto NumberOfStudents,
       List<SchoolCategoryPricingDto> CategoryPricings,
       TeamDto Team,
       List<AddressDto> Locations)
    {
        this.Id=Id;
        this.Name=Name;
        this.CompanyName=CompanyName;
        this.Email=Email;
        this.PhoneNumber=PhoneNumber;
        this.Slug=Slug;
        this.CountyId=CountyId;
        this.CityName=CityName;
        this.Slogan=Slogan;
        this.Description=Description;
        this.PhoneNumbersGroups=PhoneNumberGroups;
        this.IconData=IconData;
        this.Links=Links;
        this.BussinessHours=BussinessHours;
        this.VehicleCategories=VehicleCategories;
        this.ArrCertificates=Certificates;
        this.SchoolStatistics=NumberOfStudents;
        this.CategoryPricings=CategoryPricings;
        this.Team=Team;
        this.Locations=Locations;
    }

    private SchoolDetailReadDto() { }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string CompanyName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string Slug { get; init; }
    public string CountyId { get; init; }
    public string CityName { get; init; }
    public string Slogan { get; init; }
    public string Description { get; init; }
    public IEnumerable<PhoneNumberGroupDto> PhoneNumbersGroups { get; init; }
    public ImageReadDto IconData { get; init; }
    public WebsiteLinkRead[] Links { get; init; }
    public BussinessHours BussinessHours { get; init; }
    public List<VehicleCategoryDto> VehicleCategories { get; init; }
    public List<ArrCertificationDto>? ArrCertificates { get; init; }
    public SchoolStatisticsDto SchoolStatistics { get; init; }
    public List<SchoolCategoryPricingDto> CategoryPricings { get; init; }
    public TeamDto Team { get; init; }
    public List<AddressDto> Locations { get; init; }
}
