using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;

namespace Domain.Dtos.School;

public readonly record struct UpdateSchoolCommandDto(
    string? Name,
    string? LegalName,
    string? OwnerEmail,
    string? SchoolEmail,
    string? OwnerFirstName,
    string? OwnerLastName,
    string? City,
    string? Address,
    string? PhoneNumber,
    string? ImagePath,
    string? ImageContentType,
    List<PhoneNumberGroupDto>? PhoneNumberGroups,
    WebsiteLinkUpdateDto? WebsiteLink,
    List<SocialMediaLinkDto>? SocialMediaLinks,
    List<BusinessHoursEntryDto>? BusinessHours,
    List<string>? VehiclesCategories,
    List<string>? ArrCertifications);