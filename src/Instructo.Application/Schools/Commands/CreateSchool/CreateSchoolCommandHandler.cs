using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Instructo.Application.Abstractions.Messaging;
using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Entities;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Enums;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Mappers;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;
[assembly: InternalsVisibleTo("Instructo.UnitTests")]
namespace Instructo.Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler(
    ICommandRepository<School, SchoolId> repository,
    IQueryRepository<School, SchoolId> queryRepository,
     IQueryRepository<ArrCertificate, ARRCertificateType> certificatesRepository,
      IQueryRepository<VehicleCategory, VehicleCategoryType> vehicleQueryRepository,
    ILogger<CreateSchoolCommandHandler> logger,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    IIdentityService identityService,
    ISender sender)
    : ICommandHandler<CreateSchoolCommand, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        return await RegisterAndGetUser(request, cancellationToken)
                   .ThenAsync(user => Task.FromResult(CreateSchoolWithIcon(user, request)))
                   .ThenAsync(school => Task.FromResult(AddSchoolWebsiteLink(request, school)))
                   .ThenAsync(school => Task.FromResult(AddSocialMediaLinks(request, school)))
                   .ThenAsync(repository.AddAsync)
                   .MapAsync(school => school.ToReadDto());
    }


    private Result<School> AddSocialMediaLinks(CreateSchoolCommand request, School school)
    {
        foreach(var socialMediaLink in request.SocialMediaLinks)
        {
            if(string.IsNullOrEmpty(socialMediaLink.Url))
            {
                continue;
            }
            try
            {
                var platform = socialMediaPlatformImageProvider.Get(socialMediaLink.SocialPlatformName);
                var platformImageCreationResult = Image.Create(
              $"{request.LegalName}-{socialMediaLink.SocialPlatformName}-Icon",
              platform.IconContentType,
              platform.IconPath,
              socialMediaLink.SocialPlatformName);
                if(platformImageCreationResult.IsError)
                {
                    return Result<School>.Failure(platformImageCreationResult.Errors);
                }
                var platformImage = platformImageCreationResult.Value!;
                var linkCreationResult = WebsiteLink.Create(
                    socialMediaLink.Url,
                    socialMediaLink.SocialPlatformName,
                    $"{socialMediaLink.SocialPlatformName} {platform.Description}",
                    platformImage);
                if(linkCreationResult.IsError)
                {
                    return Result<School>.Failure(linkCreationResult.Errors);
                }
                var link = linkCreationResult.Value!;

                school.AddLink(link);
            }
            catch(ArgumentException ex)
            {
                logger.LogError(
                    ex,
                    "Error creating social media link for {SchoolName}",
                    request.Name);
                return Result<School>.Failure(
                    [new Error("Create-School", "Invalid social media platform")]);
            }

        }
        return school;
    }

    private Result<School> AddSchoolWebsiteLink(CreateSchoolCommand request, School school)
    {
        var websiteLinkIconResult = Image.Create(
              $"{request.LegalName.Value}-Website-Icon",
              request.WebsiteLink.IconData.ContentType,
              request.WebsiteLink.IconData.Url,
              "Company Website Icon");
        if(websiteLinkIconResult.IsError)
        {
            return Result<School>.Failure(websiteLinkIconResult.Errors);
        }
        var websiteLinkIcon = websiteLinkIconResult.Value!;

        var websiteLinkCreationResult = WebsiteLink.Create(
            request.WebsiteLink.Url,
            request.WebsiteLink.Name,
            request.WebsiteLink.Description,
            websiteLinkIcon);
        if(websiteLinkCreationResult.IsError)
        {
            return Result<School>.Failure(websiteLinkCreationResult.Errors);
        }
        var websiteLink = websiteLinkCreationResult.Value!;

        school.AddLink(websiteLink);
        return school;
    }

    private Result<School> CreateSchoolWithIcon(ApplicationUser user, CreateSchoolCommand request)
    {
        if(queryRepository.GetByIndexed(request.LegalName).Result.Value!.Any())
        {
            return Result<School>.Failure(
                [new Error("Create-School", "Company name already exists")]);
        }
        var iconResult = Image.Create(
                 $"{request.LegalName}-Icon",
                 request.ImageContentType,
                 request.ImagePath,
                 $"Company logo");
        if(iconResult.IsError)
            return Result<School>.Failure(iconResult.Errors);
        var icon = iconResult.Value!;

        var vehiclesCategoryRetrievalErrors = new List<Error>();
        List<VehicleCategory> selectedCategories = [];
        request.VehicleCategories.ForEach(async x =>
        {
            var categoryRequest = await vehicleQueryRepository.GetByIdAsync(x);
            if(categoryRequest.IsError)
                vehiclesCategoryRetrievalErrors.AddRange(categoryRequest.Errors);
            selectedCategories.Add(categoryRequest.Value!);
        }
        );
        if(vehiclesCategoryRetrievalErrors.Count>0)
        {
            return Result<School>.WithErrors([.. vehiclesCategoryRetrievalErrors]);
        }

        var certificatesRetrievalErrors = new List<Error>();
        var selectedCertificates = new List<ArrCertificate>();
        request.Certificates.ForEach(async certificateType =>
        {
            var certificateRequest = await certificatesRepository.GetByIdAsync(certificateType);
            if(certificateRequest.IsError)
            {
                certificatesRetrievalErrors.AddRange(certificateRequest.Errors);
            }
            selectedCertificates.Add(certificateRequest.Value!);
        });

        var school = new School(
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

        return Result<School>.Success(school);
    }

    private async Task<Result<ApplicationUser>> RegisterAndGetUser(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        var registerUserCommand = new RegisterUserCommand(
         request.OwnerFirstName,
         request.OwnerLastName,
         request.OwnerEmail,
         request.OwnerPassword,
         request.PhoneNumber,
         "Owner");
        var userRegistrationRequest = await sender.Send(
            registerUserCommand, cancellationToken);
        if(userRegistrationRequest.IsError)
        {
            return Result<ApplicationUser>.Failure(userRegistrationRequest.Errors);
        }
        var user = await identityService.GetUserByEmailAsync(request.OwnerEmail);
        if(user is null)
        {
            return Result<ApplicationUser>.Failure([new Error(
                "Create-School",
                "User retrieval failed after registration")]);
        }
        return user;
    }
}