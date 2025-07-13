using Application.Abstractions.Messaging;
using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Commands.UpdateSchool;

public partial record UpdateSchoolCommand : ICommand<Result<SchoolDetailReadDto>>
{
    private UpdateSchoolCommand()
    {
    }

    public SchoolId SchoolId { get; internal set; }
    public Guid RequestingUserId { get; internal set; }
    public SchoolName? Name { get; internal set; }
    public LegalName? LegalName { get; internal set; }
    public Email? SchoolEmail { get; internal set; }
    public Email? OwnerEmail { get; internal set; }
    public Name? OwnerFirstName { get; internal set; }
    public Name? OwnerLastName { get; internal set; }
    public CityDto? City { get; internal set; }
    public AddressDto? Address { get; internal set; }
    public PhoneNumber? PhoneNumber { get; internal set; } = PhoneNumber.Empty;
    public List<PhoneNumbersGroup>? PhoneNumbersGroups { get; internal set; } = [];
    public FilePath? ImagePath { get; internal set; }
    public ContentType? ImageContentType { get; internal set; }
    public WebsiteLinkUpdateDto? WebsiteLink { get; internal set; }
    public List<SocialMediaLinkDto>? SocialMediaLinks { get; internal set; } = [];
    public BussinessHours? BussinessHours { get; internal set; } = BussinessHours.Empty;
    public List<VehicleCategoryType>? VehicleCategories { get; internal set; } = [];
    public List<ARRCertificateType>? Certificates { get; internal set; } = [];
}