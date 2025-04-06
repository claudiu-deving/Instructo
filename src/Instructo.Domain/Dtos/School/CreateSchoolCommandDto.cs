using Instructo.Domain.Dtos.Link;
using Instructo.Domain.Dtos.PhoneNumbers;

namespace Instructo.Domain.Dtos.School;

public readonly record struct CreateSchoolCommandDto(
    string Name,
    string LegalName,
    string OwnerEmail,
    string SchoolEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName,
    string City,
    string Address,
    string? PhoneNumber,
    string ImagePath,
    string ImageContentType,
    List<PhoneNumberGroupDto> PhoneNumberGroups,
    WebsiteLinkReadDto WebsiteLink,
    List<SocialMediaLinkDto> SocialMediaLinks,
    List<BussinessHoursEntryDto> BussinessHours,
    List<string> VechiclesCategories,
    List<string> ArrCertifications);