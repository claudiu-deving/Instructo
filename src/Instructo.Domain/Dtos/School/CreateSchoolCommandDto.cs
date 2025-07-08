using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;

namespace Domain.Dtos.School;

public readonly record struct CreateSchoolCommandDto(
    string Name,
    string LegalName,
    string OwnerEmail,
    string SchoolEmail,
    string City,
    string Address,
    string? PhoneNumber,
    string ImagePath,
    string ImageContentType,
    string? Slogan,
    string Description,
    string X,
    string Y,
    List<PhoneNumberGroupDto> PhoneNumberGroups,
    WebsiteLinkReadDto WebsiteLink,
    List<SocialMediaLinkDto> SocialMediaLinks,
    List<BusinessHoursEntryDto> BussinessHours,
    List<string> VechiclesCategories,
    List<string> ArrCertifications);