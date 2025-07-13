using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

namespace Domain.Dtos.School;

public readonly record struct SchoolDetailReadDto(
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
    string StreetAndNumber,
    double Longitude,
       double Latitude,
    IEnumerable<PhoneNumberGroupDto> PhoneNumberGroups,
    ImageReadDto IconData,
    WebsiteLinkReadDto[] Links,
    List<BussinessHoursEntry> BussinessHours,
    List<VehicleCategoryDto> VehicleCategories,
    List<ArrCertificationDto> Certificates);
public readonly record struct SchoolListReadDto(
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
    string StreetAndNumber,
    ImageReadDto IconData
  );
