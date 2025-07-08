using System.Runtime.CompilerServices;

using Application.Abstractions.Messaging;
using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.GetUserByEmail;
using Application.Users.Queries.GetUserById;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;

using MediatR;

using Microsoft.AspNetCore.Identity;

[assembly: InternalsVisibleTo("Instructo.IntegrationTests")]

namespace Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler(
    ISchoolManagementDirectory schoolManagementDirectory,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    RoleManager<ApplicationRole> roleManager,
    ISender sender) : ICommandHandler<CreateSchoolCommand, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        return await FlexContext.StartContextAsync(request)
            .Then(GetUser)
            .Then(CreateSchoolWithIcon)
            .Then(AddSchoolWebsiteLink)
            .Then(AddSocialMediaLinks)
            .Then(ctx => schoolManagementDirectory.SchoolCommandRepository.AddAsync(ctx.Get<School>()))
            .MapAsync(ctx =>
            {
                ctx.Get<ApplicationUser>().Role=roleManager.Roles.FirstOrDefault(x => x.Name=="Owner");
                schoolManagementDirectory.SaveChangesAsync();

                return ctx.Get<School>().ToReadDto();
            });
    }

    private Result<School> AddSocialMediaLinks(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var school = context.Get<School>();
        var errors = new List<Error>();
        foreach(var socialMediaLink in request.SocialMediaLinks)
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
            .Then(GetCity)
            .Then(CreateAddress)
            .Then(CreateVehicleCategories)
            .Then(CreateCerfificates)
            .MapAsync(CreateSchool);

        async Task<Result<CreateSchoolCommand>> CheckCompanyName(FlexContext _)
        {
            return (await schoolManagementDirectory.SchoolQueriesRepository.GetByIndexed(request.LegalName))?.Value is not null
                ? Result<CreateSchoolCommand>.Failure(
                    new Error("Create-School", "Company name already exists"))
                : Result<CreateSchoolCommand>.Success(request);
        }

        async Task<Result<City>> GetCity(FlexContext _)
        {
            return (await schoolManagementDirectory.CityQueriesRepository.GetByIndexed(request.City.Value))?.Value is not City fromDb
                ? Result<City>.Failure(
                    new Error("Create-School", $"City {request.City.Value} cannot be found"))
                : Result<City>.Success(fromDb);
        }

        Result<Address> CreateAddress(FlexContext _)
        {
            return Address.Create(
                request.Address.Street,
                request.Address.Longitude,
                request.Address.Latitude);
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

        async Task<Result<List<VehicleCategory>>> CreateVehicleCategories(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();

            // Single query to get all categories at once
            var categoriesResult = await schoolManagementDirectory.VehicleQueriesRepository.GetAllAsync();

            if(categoriesResult.IsError)
                return Result<List<VehicleCategory>>.WithErrors(categoriesResult.Errors);

            var foundCategories = categoriesResult.Value;
            var foundIds = foundCategories.Select(c => c.Id).ToHashSet();
            var missingIds = request.VehicleCategories.Except(foundIds).ToList();

            if(missingIds.Count!=0)
            {
                var errors = missingIds.Select(id =>
                    new Error("Category-Ids-Match", $"Vehicle category with ID {id} not found")).ToList();
                return Result<List<VehicleCategory>>.WithErrors([.. errors]);
            }

            return foundCategories.Where(x => request.VehicleCategories.Contains(x.Id)).ToList();
        }

        Result<List<ArrCertificate>> CreateCerfificates(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            var certificatesRetrievalErrors = new List<Error>();
            var selectedCertificates = new List<ArrCertificate>();
            request.Certificates.ForEach(async certificateType =>
            {
                await FlexContext.StartContextAsync()
                    .Then(ctx => schoolManagementDirectory.CertificateQueriesRepository.GetByIdAsync(certificateType))
                    .FinalizeContext(ctx => selectedCertificates.Add(ctx.Get<ArrCertificate>()));
            });
            if(certificatesRetrievalErrors.Count!=0)
                return Result<List<ArrCertificate>>.WithErrors([.. certificatesRetrievalErrors]);
            return selectedCertificates;
        }

        static School CreateSchool(FlexContext flexContext)
        {
            var user = flexContext.Get<ApplicationUser>();
            var request = flexContext.Get<CreateSchoolCommand>();
            var selectedCategories = flexContext.Get<List<VehicleCategory>>();
            var selectedCertificates = flexContext.Get<List<ArrCertificate>>();
            var city = flexContext.Get<Domain.Entities.City>();
            var icon = flexContext.Get<Image>();
            var address = flexContext.Get<Address>();
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
                icon,
                city,
                request.Slogan,
                request.Description,
                address
            );
        }
    }

    private async Task<Result<ApplicationUser>> GetUser(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var getUserByEmail = new GetUserByEmailQuery(request.OwnerEmail);

        var owner = await sender.Send(getUserByEmail);
        if(owner.IsError)
        {
            return Result<ApplicationUser>.Failure(
                new Error("Create-School", $"Owner with email {request.OwnerEmail} not found"));
        }
        if(owner.Value is null)
        {
            return Result<ApplicationUser>.Failure(
                new Error("Create-School", $"Owner with email {request.OwnerEmail} not found"));
        }
        if(owner.Value!.School is not null)
        {
            return Result<ApplicationUser>.Failure(
                new Error("Create-School-Already-Owner", $"Owner with email {request.OwnerEmail} already has a school"));
        }
        return owner;
    }
}