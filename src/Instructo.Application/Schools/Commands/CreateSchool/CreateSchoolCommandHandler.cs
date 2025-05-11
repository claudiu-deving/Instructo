using System.Runtime.CompilerServices;
using Application.Abstractions.Messaging;
using Application.Users.Commands.RegisterUser;
using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;
using MediatR;

[assembly: InternalsVisibleTo("Instructo.UnitTests")]

namespace Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler(
    ISchoolCommandRepository repository,
    IQueryRepository<School, SchoolId> queryRepository,
    IQueryRepository<ArrCertificate, ARRCertificateType> certificatesRepository,
    IQueryRepository<VehicleCategory, VehicleCategoryType> vehicleQueryRepository,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    ISender sender)
    : ICommandHandler<CreateSchoolCommand, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        return await FlexContext.StartContextAsync(request)
            .Then(RegisterAndGetUser)
            .Then(CreateSchoolWithIcon)
            .Then(AddSchoolWebsiteLink)
            .Then(AddSocialMediaLinks)
            .Then(ctx => repository.AddAsync(ctx.Get<School>()))
            .MapAsync(ctx => ctx.Get<School>().ToReadDto());
    }

    private Result<School> AddSocialMediaLinks(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var school = context.Get<School>();
        var errors = new List<Error>();
        foreach (var socialMediaLink in request.SocialMediaLinks)
            FlexContext.StartContext(request, school, socialMediaLink)
                .Then(ctx => socialMediaPlatformImageProvider.Get(socialMediaLink.SocialPlatformName))
                .Then(CreateImage)
                .Then(CreateSocialMediaLink)
                .MapContext(ctx => school.AddLink(ctx.Get<WebsiteLink>()));
        return school;

        static Result<WebsiteLink> CreateSocialMediaLink(FlexContext flexContext)
        {
            var request = flexContext.Get<CreateSchoolCommand>();
            var socialMediaLink = flexContext.Get<SocialMediaLinkDto>();
            var platform = flexContext.Get<SocialMediatPlatform>();
            var platformImage = flexContext.Get<Image>();
            return WebsiteLink.Create(
                socialMediaLink.Url,
                socialMediaLink.SocialPlatformName,
                $"{socialMediaLink.SocialPlatformName} {platform.Description}",
                platformImage);
        }

        static Result<Image> CreateImage(FlexContext flexContext)
        {
            var request = flexContext.Get<CreateSchoolCommand>();
            var socialMediaLink = flexContext.Get<SocialMediaLinkDto>();
            var platform = flexContext.Get<SocialMediatPlatform>();
            return Image.Create(
                $"{request.LegalName}-{socialMediaLink.SocialPlatformName}-Icon",
                platform.IconContentType,
                platform.IconPath,
                socialMediaLink.SocialPlatformName);
        }
    }

    private static Result<School> AddSchoolWebsiteLink(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var school = context.Get<School>();

        return FlexContext.StartContext(request, school)
            .Then(CreateImage)
            .Then(CreateWebsiteLink)
            .MapContext(ctx => school.AddLink(ctx.Get<WebsiteLink>()));

        static Result<Image> CreateImage(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            return Image.Create(
                $"{request.LegalName.Value}-Website-Icon",
                request.WebsiteLink.IconData.ContentType,
                request.WebsiteLink.IconData.Url,
                "Company Website Icon");
        }

        static Result<WebsiteLink> CreateWebsiteLink(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            var websiteLinkIcon = context.Get<Image>();
            return WebsiteLink.Create(
                request.WebsiteLink.Url,
                request.WebsiteLink.Name,
                request.WebsiteLink.Description,
                websiteLinkIcon);
        }
    }

    private async Task<Result<School>> CreateSchoolWithIcon(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var user = context.Get<ApplicationUser>();
        return await FlexContext.StartContextAsync(request, user)
            .Then(CheckCompanyName)
            .Then(CreateImage)
            .Then(CreateVehicleCategories)
            .Then(CreateCerfificates)
            .MapAsync(CreateSchool);

        async Task<Result<CreateSchoolCommand>> CheckCompanyName(FlexContext _)
        {
            return (await queryRepository.GetByIndexed(request.LegalName))?.Value is not null
                ? Result<CreateSchoolCommand>.Failure(
                    new Error("Create-School", "Company name already exists"))
                : Result<CreateSchoolCommand>.Success(request);
        }

        static Result<Image> CreateImage(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            return Image.Create(
                $"{request.LegalName}-Icon",
                request.ImageContentType,
                request.ImagePath,
                "Company logo");
        }

        Result<List<VehicleCategory>> CreateVehicleCategories(FlexContext _)
        {
            var request = context.Get<CreateSchoolCommand>();
            var vehiclesCategoryRetrievalErrors = new List<Error>();
            List<VehicleCategory> selectedCategories = [];
            request.VehicleCategories.ForEach(async x =>
                {
                    var categoryRequest = await vehicleQueryRepository.GetByIdAsync(x);
                    if (categoryRequest.IsError)
                        vehiclesCategoryRetrievalErrors.AddRange(categoryRequest.Errors);
                    selectedCategories.Add(categoryRequest.Value!);
                }
            );
            if (vehiclesCategoryRetrievalErrors.Count > 0)
                return Result<List<VehicleCategory>>.WithErrors([.. vehiclesCategoryRetrievalErrors]);

            return Result<List<VehicleCategory>>.Success(selectedCategories);
        }

        Result<List<ArrCertificate>> CreateCerfificates(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            var certificatesRetrievalErrors = new List<Error>();
            var selectedCertificates = new List<ArrCertificate>();
            request.Certificates.ForEach(async certificateType =>
            {
                await FlexContext.StartContextAsync()
                    .Then(ctx => certificatesRepository.GetByIdAsync(certificateType))
                    .FinalizeContext(ctx => selectedCertificates.Add(ctx.Get<ArrCertificate>()));
            });
            if (certificatesRetrievalErrors.Count != 0)
                return Result<List<ArrCertificate>>.WithErrors([.. certificatesRetrievalErrors]);
            return selectedCertificates;
        }

        static School CreateSchool(FlexContext flexContext)
        {
            var user = flexContext.Get<ApplicationUser>();
            var request = flexContext.Get<CreateSchoolCommand>();
            var selectedCategories = flexContext.Get<List<VehicleCategory>>();
            var selectedCertificates = flexContext.Get<List<ArrCertificate>>();
            var icon = flexContext.Get<Image>();
            return new School(
                user,
                request.Name,
                request.LegalName,
                request.SchoolEmail,
                request.PhoneNumber,
                request.PhoneNumbersGroups,
                request.BussinessHours,
                selectedCategories,
                selectedCertificates,
                icon);
        }
    }

    private async Task<Result<ApplicationUser>> RegisterAndGetUser(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var registerUserCommand = new RegisterUserCommand(
            request.OwnerFirstName,
            request.OwnerLastName,
            request.OwnerEmail,
            request.OwnerPassword,
            request.PhoneNumber,
            "Owner");

        return await sender.Send(registerUserCommand);
    }
}