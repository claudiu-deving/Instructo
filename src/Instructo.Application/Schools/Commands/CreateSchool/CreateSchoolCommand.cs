using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Dtos.Link;
using Instructo.Domain.Dtos.PhoneNumbers;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Enums;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Application.Schools.Commands.CreateSchool;

public partial record CreateSchoolCommand : ICommand<Result<SchoolReadDto>>
{
    private CreateSchoolCommand()
    {
    }
    public SchoolName Name { get; internal set; }
    public LegalName LegalName { get; internal set; }
    public Email SchoolEmail { get; internal set; }
    public Email OwnerEmail { get; internal set; }
    public Password OwnerPassword { get; internal set; }
    public Name OwnerFirstName { get; internal set; }
    public Name OwnerLastName { get; internal set; }
    public City City { get; internal set; }
    public Address Address { get; internal set; }
    public PhoneNumber PhoneNumber { get; internal set; } = PhoneNumber.Empty;
    public List<PhoneNumbersGroup> PhoneNumbersGroups { get; internal set; } = [];
    public FilePath ImagePath { get; internal set; }
    public ContentType ImageContentType { get; internal set; }
    public WebsiteLinkReadDto WebsiteLink { get; internal set; }
    public List<SocialMediaLinkDto> SocialMediaLinks { get; internal set; } = [];
    public BussinessHours BussinessHours { get; internal set; } = BussinessHours.Empty;
    public List<VehicleCategoryType> VehicleCategories { get; internal set; } = [];
    public List<ARRCertificateType> Certificates { get; internal set; } = [];
}