using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserById;

using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

using Messager;

namespace Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandHandler(
    ISchoolCommandRepository repository,
   ISchoolQueriesRepository queryRepository,
    IQueryRepository<ArrCertificate, int> certificatesRepository,
    IQueryRepository<VehicleCategory, int> vehicleQueryRepository,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    ISender sender) : ICommandHandler<UpdateSchoolCommand, Result<SchoolDetailReadDto>>
{
    public async Task<Result<SchoolDetailReadDto>> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        return await FlexContext.StartContextAsync(request)
            .Then(GetSchool)
            .Then(GetOwner)
            .Then(UpdateSchoolData)
            .Then(UpdateSchoolWebsiteLink)
            .Then(UpdateSocialMediaLinks)
            .Then(ctx => repository.UpdateAsync(ctx.Get<School>()))
            .MapAsync(ctx => ctx.Get<School>().ToDetailedReadDto());
    }

    private async Task<Result<School>> GetSchool(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = await queryRepository.GetByIdAsync(request.SchoolId);
        return !existingSchool.IsError
            ? existingSchool!
            : Result<School>.Failure([.. existingSchool.Errors, new Error("Not-Found", "Unable to retrieve school")]);
    }

    private async Task<Result<ApplicationUser>> GetOwner(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var registerUserCommand = new GetUserByIdQuery(request.RequestingUserId);
        return await sender.Send(registerUserCommand);
    }

    private async Task<Result<School>> UpdateSchoolData(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var user = context.Get<ApplicationUser>();
        var existingSchool = context.Get<School>();
        return await FlexContext.StartContextAsync(request, user, existingSchool)
            .Then(ChangeSchoolName)
            .Then(CheckCompanyName)
            .Then(UpdateImage)
            .Then(UpdateVehicleCategories)
            .Then(UpdateCertificates)
            .MapAsync(ctx => ctx.Get<School>());

        Result<UpdateSchoolCommand> ChangeSchoolName(FlexContext localContext)
        {
            if(request.Name is null)
                return Result<UpdateSchoolCommand>.Success(request);

            return SchoolName.Create(request.Name).Match(
                name =>
                {
                    existingSchool.ChangeName(name);
                    return Result<UpdateSchoolCommand>.Success(request);
                },
                errors => Result<UpdateSchoolCommand>.WithErrors([.. errors]));
        }


        async Task<Result<UpdateSchoolCommand>> CheckCompanyName(FlexContext localContext)
        {
            if(request.LegalName is null)
                return Result<UpdateSchoolCommand>.Success(request);
            if((await queryRepository.GetByIndexed(request.LegalName)).Value is not null)
                return Result<UpdateSchoolCommand>.Failure(new Error("Create-School", "Company name already exists"));

            return Result<UpdateSchoolCommand>.Success(request);
        }

        Result<Image> UpdateImage(FlexContext localContext)
        {
            var existingImage = existingSchool.Icon;
            if(existingImage is null)
                throw new NullReferenceException($"The {existingSchool.Id} doesn't contain any image");
            if(request.LegalName is null
                ||request.ImageContentType is null
                ||request.ImagePath is null)
                return Result<Image>.Success(existingImage);
            return Image.Create(
                $"{request.LegalName}-Icon",
                request.ImageContentType,
                request.ImagePath,
                "Company logo");
        }

        Result<List<VehicleCategory>> UpdateVehicleCategories(FlexContext localContext)
        {
            var existingCategories = existingSchool.VehicleCategories;
            if(request.VehicleCategories is null||request.VehicleCategories.Count==0)
                return Result<List<VehicleCategory>>.Success(existingCategories.ToList());
            var vehiclesCategoryRetrievalErrors = new List<Error>();
            List<VehicleCategory> selectedCategories = [];
            request.VehicleCategories.ForEach(async void (x) =>
                {
                    try
                    {
                        await FlexContext.StartContextAsync()
                            .Then(_ => vehicleQueryRepository.GetByIdAsync((int)x))
                            .FinalizeContext(ctx => selectedCategories.Add(ctx.Get<VehicleCategory>()));
                    }
                    catch(Exception e)
                    {
                        vehiclesCategoryRetrievalErrors.Add(new Error("VC-Retrieval-Update", e.Message));
                    }
                }
            );
            if(selectedCategories.Count!=0)
            {
                existingCategories.ToList().ForEach(category => existingSchool.RemoveVehicleCategory(category));
                selectedCategories.ForEach(existingSchool.AddVehicleCategory);
            }

            return vehiclesCategoryRetrievalErrors.Count>0
                ? Result<List<VehicleCategory>>.WithErrors([.. vehiclesCategoryRetrievalErrors])
                : Result<List<VehicleCategory>>.Success(selectedCategories);
        }

        Result<List<ArrCertificate>> UpdateCertificates(FlexContext localContext)
        {
            var certificatesRetrievalErrors = new List<Error>();
            var selectedCertificates = new List<ArrCertificate>();
            var existingCertificates = existingSchool.Certificates;
            if(request.Certificates is null||request.Certificates.Count==0)
                return Result<List<ArrCertificate>>.Success(existingCertificates.ToList());
            request.Certificates.ForEach(async void (certificateType) =>
            {
                try
                {
                    await FlexContext.StartContextAsync()
                        .Then(_ => certificatesRepository.GetByIdAsync((int)certificateType))
                        .FinalizeContext(ctx => selectedCertificates.Add(ctx.Get<ArrCertificate>()));
                }
                catch(Exception e)
                {
                    certificatesRetrievalErrors.Add(new Error("Cert-Retrieval-Update", e.Message));
                }
            });

            if(selectedCertificates.Count!=0)
            {
                existingCertificates.ToList().ForEach(certificate => existingSchool.RemoveCertificate(certificate));
                selectedCertificates.ForEach(existingSchool.AddCertificate);
            }

            return certificatesRetrievalErrors.Count!=0
                ? Result<List<ArrCertificate>>.WithErrors([.. certificatesRetrievalErrors])
                : selectedCertificates;
        }
    }


    private Result<School> UpdateSocialMediaLinks(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var school = context.Get<School>();
        if(request.SocialMediaLinks is null)
            return school;
        foreach(var socialMediaLink in request.SocialMediaLinks)
            FlexContext.StartContext(request, school, socialMediaLink)
                .Then(_ => socialMediaPlatformImageProvider.Get(socialMediaLink.SocialPlatformName))
                .Then(CreateImage)
                .Then(CreateSocialMediaLink)
                .MapContext(ctx => school.AddLink(ctx.Get<WebsiteLink>()));
        return school;

        static Result<WebsiteLink> CreateSocialMediaLink(FlexContext flexContext)
        {
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
            var request = flexContext.Get<UpdateSchoolCommand>();
            var socialMediaLink = flexContext.Get<SocialMediaLinkDto>();
            var platform = flexContext.Get<SocialMediatPlatform>();
            return Image.Create(
                $"{request.LegalName}-{socialMediaLink.SocialPlatformName}-Icon",
                platform.IconContentType,
                platform.IconPath,
                socialMediaLink.SocialPlatformName);
        }
    }

    private static Result<School> UpdateSchoolWebsiteLink(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = context.Get<School>();
        var existingWebsite =
            existingSchool.WebsiteLinks.FirstOrDefault(link => link.Description=="Company Website Icon");
        if(existingWebsite is null||request.WebsiteLink is null)
            return existingSchool;
        return FlexContext.StartContext(request, existingSchool)
            .Then(UpdateImage)
            .Then(CreateWebsiteLink)
            .MapContext(ctx => existingSchool.AddLink(ctx.Get<WebsiteLink>()));

        Result<Image> UpdateImage(FlexContext localContext)
        {
            string legalName = request.LegalName??existingSchool.CompanyName;
            var existingImage = existingWebsite.Icon??throw new NullReferenceException("Unable to find icon of existing school website");

            if(request.WebsiteLink?.IconData is null)
                return existingImage;

            return existingImage.Update($"{legalName}-Website-Icon",
                request.WebsiteLink.Value.IconData.Value.ContentType,
                request.WebsiteLink.Value.IconData.Value.Url,
                "Company Website Icon"
            );
        }

        Result<WebsiteLink> CreateWebsiteLink(FlexContext localContext)
        {
            var websiteLinkIcon = localContext.Get<Image>();

            return existingWebsite.Update(request.WebsiteLink.Value.Url, request.WebsiteLink.Value.Name,
                request.WebsiteLink.Value.Description, websiteLinkIcon);
        }
    }
}