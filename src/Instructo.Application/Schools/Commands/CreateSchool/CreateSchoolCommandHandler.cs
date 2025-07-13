using System.Runtime.CompilerServices;

using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserByEmail;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;

using Messager;

using Microsoft.AspNetCore.Identity;

[assembly: InternalsVisibleTo("Instructo.IntegrationTests")]

namespace Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler(
    ISchoolManagementDirectory schoolManagementDirectory,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    RoleManager<ApplicationRole> roleManager,
    ISender sender) : ICommandHandler<CreateSchoolCommand, Result<SchoolDetailReadDto>>
{
    public async Task<Result<SchoolDetailReadDto>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        var result = await FlexContext.StartContextAsync(request)
            .Then(GetUser)
            .Then(CreateSchoolWithIcon)
            .Then(AddSchoolWebsiteLink)
            .Then(AddSocialMediaLinks)
            .Then(ctx => schoolManagementDirectory.SchoolCommandRepository.AddAsync(ctx.Get<School>()));

        if(result.IsError)
            return Result<SchoolDetailReadDto>.Failure(result.Errors);

        var ctx = result.Value!;
        var user = ctx.Get<ApplicationUser>();
        var ownerRole = roleManager.Roles.FirstOrDefault(x => x.Name=="Owner");
        if(ownerRole!=null)
        {
            user.Role=ownerRole;
        }
        await schoolManagementDirectory.SaveChangesAsync();

        return Result<SchoolDetailReadDto>.Success(ctx.Get<School>().ToReadDto());
    }

    private Result<School> AddSocialMediaLinks(FlexContext context)
    {
        var request = context.Get<CreateSchoolCommand>();
        var school = context.Get<School>();
        var errors = new List<Error>();

        foreach(var socialMediaLink in request.SocialMediaLinks)
        {
            var result = FlexContext.StartContext(request, school, socialMediaLink)
                .Then(ctx => socialMediaPlatformImageProvider.Get(socialMediaLink.SocialPlatformName))
                .Then(CreateImage)
                .Then(CreateSocialMediaLink)
                .MapContext(ctx => school.AddLink(ctx.Get<WebsiteLink>()));

            if(result.IsError)
            {
                errors.AddRange(result.Errors);
            }
        }

        if(errors.Count>0)
        {
            return Result<School>.Failure([.. errors]);
        }

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
            .Then(CreateCertificates)
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
            try
            {
                return Address.Create(
                    request.Address.Street,
                    request.Address.Longitude,
                    request.Address.Latitude);
            }
            catch(Exception ex)
            {
                return Result<Address>.Failure(
                    new Error("Create-School-Address", $"Failed to create address: {ex.Message}"));
            }
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
            var missingIds = request.VehicleCategories.Select(x => (int)x).Except(foundIds).ToList();

            if(missingIds.Count!=0)
            {
                var errors = missingIds.Select(id =>
                    new Error("Category-Ids-Match", $"Vehicle category with ID {id} not found")).ToList();
                return Result<List<VehicleCategory>>.WithErrors([.. errors]);
            }

            return foundCategories.Where(x => request.VehicleCategories.Contains((VehicleCategoryType)x.Id)).ToList();
        }

        async Task<Result<List<ArrCertificate>>> CreateCertificates(FlexContext context)
        {
            var request = context.Get<CreateSchoolCommand>();
            var certificatesRetrievalErrors = new List<Error>();
            var selectedCertificates = new List<ArrCertificate>();

            foreach(var certificateType in request.Certificates)
            {
                var certificateResult = await schoolManagementDirectory.CertificateQueriesRepository.GetByIdAsync((int)certificateType);
                if(certificateResult.IsError)
                {
                    certificatesRetrievalErrors.AddRange(certificateResult.Errors);
                }
                else if(certificateResult.Value is not null)
                {
                    selectedCertificates.Add(certificateResult.Value);
                }
                else
                {
                    certificatesRetrievalErrors.Add(new Error("Certificate-Not-Found", $"Certificate with type {certificateType} not found"));
                }
            }

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
            return School.Create(
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