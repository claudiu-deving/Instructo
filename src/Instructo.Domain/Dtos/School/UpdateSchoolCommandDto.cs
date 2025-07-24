using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.ValueObjects;

namespace Domain.Dtos.School;

public readonly record struct UpdateSchoolCommandDto(
    string? Name,
    string? LegalName,
    string? SchoolEmail,
    string? City,
    string? Slogan,
    string? Description,
    string? PhoneNumber,
    string? ImagePath,
    string? ImageContentType,
    string? MainLocationStreet,
    string? MainLocationLongitude,
    string? MainLocationLatitude,
    List<PhoneNumberGroupDto>? PhoneNumberGroups,
    WebsiteLinkUpdateDto? WebsiteLink,
    List<SocialMediaLinkDto>? SocialMediaLinks,
    List<BusinessHoursEntryDto>? BusinessHours,
    List<string>? VehiclesCategories,
    List<string>? ArrCertifications,
    int? NumberOfStudents,
    List<SchoolCategoryPricingDto>? CategoryPricings,
    List<AddressDto>? ExtraLocations,
    TeamDto? Team);