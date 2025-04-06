using Instructo.Domain.Dtos.Image;
using Instructo.Domain.Dtos.Link;
using Instructo.Domain.Dtos.PhoneNumbers;
using Instructo.Domain.Entities;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Dtos.School;

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
