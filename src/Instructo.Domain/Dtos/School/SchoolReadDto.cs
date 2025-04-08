using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.ValueObjects;

namespace Domain.Dtos.School;

public readonly record struct SchoolReadDto(
    Guid Id,
    string Name,
    string CompanyName,
    string Email,
    string PhoneNumber,
    IEnumerable<PhoneNumberGroupDto> PhoneNumberGroups,
    ImageReadDto IconData,
    WebsiteLinkReadDto[] Links,
    List<BussinessHoursEntry> BussinessHours,
    List<VehicleCategoryDto> VehicleCategories,
    List<ArrCertificationDto> Certificates);
