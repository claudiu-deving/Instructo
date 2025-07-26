using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserById;

using Domain.Dtos;
using Domain.Dtos.Link;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Models;
using Domain.Shared;
using Domain.ValueObjects;

using Messager;

namespace Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandHandler(
    ISchoolManagementDirectory schoolManagementDirectory,
    ISocialMediaPlatformImageProvider socialMediaPlatformImageProvider,
    ICommandRepository<Image, ImageId> imageCommandRepository,
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
            .Then(UpdateCategoryPricings)
            .Then(UpdateLocations)
            .Then(UpdateTeam)
            .Then(async ctx =>
            {
                var result = await schoolManagementDirectory.SchoolCommandRepository.UpdateAsync(ctx.Get<School>());
                if(!result.IsError)
                {
                    await schoolManagementDirectory.SaveChangesAsync();
                }
                return result;
            })
            .MapAsync(ctx => ctx.Get<School>().ToDetailedReadDto());
    }

    private async Task<Result<object>> UpdateTeam(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = context.Get<School>();
        if(request.Team is null)
            return existingSchool;
        var existingTeam = existingSchool.Team;
        existingTeam??=existingSchool.CreateTeam();

        var teamMemberErrors = new List<Error>();
        if(existingTeam!.Instructors.Count==0&&request.Team.Value!.Instructors.Count==0)
            return existingSchool;
        var vehicleCategories = await schoolManagementDirectory.VehicleQueriesRepository.GetAllAsync();
        if(vehicleCategories.IsError)
        {
            teamMemberErrors.AddRange(vehicleCategories.Errors);
            return Result<object>.WithErrors([.. teamMemberErrors]);
        }
        foreach(var instructorDto in request.Team.Value!.Instructors)
        {
            if(instructorDto.Id is null)
            {
                await CreateAndInsertInstructor(existingTeam, teamMemberErrors, vehicleCategories, instructorDto);
                continue;
            }
            var existingInstructor = await schoolManagementDirectory.InstructorProfileQueryRepository.GetByIdAsync((Guid)instructorDto.Id);
            if(existingInstructor.IsError)
            {
                teamMemberErrors.AddRange(existingInstructor.Errors);
                continue;
            }
            if(existingInstructor.Value is null)
            {
                teamMemberErrors.Add(new Error("Instructor-Not-Found", $"Instructor with id {instructorDto.Id} not found"));
                continue;
            }
            existingInstructor.Value.UpdateProfile(
                instructorDto.FirstName,
                instructorDto.LastName,
                DateTime.UtcNow.Year-instructorDto.Age,
                instructorDto.YearsExperience,
                instructorDto.Specialization??"",
                instructorDto.Description,
                instructorDto.PhoneNumber,
                instructorDto.Email,
                null
                );
        }

        return teamMemberErrors.Count!=0
            ? Result<object>.WithErrors([.. teamMemberErrors])
            : existingSchool;
    }

    private async Task CreateAndInsertInstructor(Team existingTeam, List<Error> teamMemberErrors, Result<IEnumerable<VehicleCategory>> vehicleCategories, InstructorDto instructorDto)
    {
        await UpsertImageIfNeeded(teamMemberErrors, instructorDto);

        var instructorVehicleCategories = GetInstructorVehicleCategories(instructorDto.Categories, vehicleCategories.Value!);

        var instructor = InstructorProfile.Create(
               instructorDto.FirstName,
               instructorDto.LastName,
               DateTime.UtcNow.Year-instructorDto.Age,
               instructorDto.YearsExperience,
               instructorDto.Specialization??"",
               instructorDto.Description,
               instructorDto.PhoneNumber,
               instructorDto.Email,
               instructorDto.Gender,
               null,
               instructorVehicleCategories
               );
        existingTeam.AddInstructor(instructor);
    }

    private List<VehicleCategory> GetInstructorVehicleCategories(List<VehicleCategoryDto> categories, IEnumerable<VehicleCategory> vehicleCategories)
    {
        var instructorVehicleCategories = new List<VehicleCategory>();
        foreach(var category in categories)
        {
            var vehicleCategory = vehicleCategories.FirstOrDefault(x => x.Name==category.Name);
            if(vehicleCategory is not null)
                instructorVehicleCategories.Add(vehicleCategory);
        }
        return instructorVehicleCategories;
    }

    private async Task UpsertImageIfNeeded(List<Error> teamMemberErrors, InstructorDto instructorDto)
    {
        if(instructorDto.ProfileImageContentType is not null
           &&instructorDto.ProfileImageName is not null
           &&instructorDto.ProfileImageContentType is not null)
        {
            //TODO: Check if image already exists, if so, update it, to be done after the whole image integration service

        }
    }

    private Result<School> UpdateLocations(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = context.Get<School>();
        if(request.ExtraLocations is null)
            return existingSchool;
        var extraLocationsErrors = new List<Error>();
        existingSchool.Locations.Where(x => x.AddressType!=AddressType.MainLocation).ToList().ForEach(x => existingSchool.RemoveExtraLocation(x));

        //Bulk update all at once, clear and the add them back
        request.ExtraLocations.ForEach(locationDto =>
        {
            try
            {
                var address = Address.Create(locationDto.Street, locationDto.Latitude, locationDto.Longitude, AddressType.LessonStart, locationDto.Comment);

                if(address.IsError)
                {
                    extraLocationsErrors.AddRange(address.Errors);
                    return;
                }

                existingSchool.AddExtraLocation(address.Value!);
            }
            catch(Exception e)
            {
                extraLocationsErrors.Add(new Error("Extra-Location-Update", e.Message));
            }
        });

        if(request.ExtraLocations.Any(x => x.AddressType==AddressType.MainLocation))
        {
            var existingMainLocation = existingSchool.Locations.FirstOrDefault(x => x.AddressType==AddressType.MainLocation);

            existingSchool.RemoveExtraLocation(existingMainLocation!);
            var mainLocationDto = request.ExtraLocations.FirstOrDefault(x => x.AddressType==AddressType.MainLocation);
            var newMainLocation = Address.Create(
                mainLocationDto!.Street,
                mainLocationDto.Latitude,
                mainLocationDto.Longitude,
                AddressType.MainLocation,
                mainLocationDto.Comment);
            if(newMainLocation.IsError)
            {
                extraLocationsErrors.AddRange(newMainLocation.Errors);
            }
            else
            {
                existingSchool.AddExtraLocation(newMainLocation.Value!);
            }
        }

        return extraLocationsErrors.Count!=0
            ? Result<School>.WithErrors([.. extraLocationsErrors])
            : existingSchool;
    }

    private Result<School> UpdateCategoryPricings(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = context.Get<School>();
        if(request.CategoryPricings is null)
            return existingSchool;
        var vehicleCategoryPricingErrors = new List<Error>();
        var vehicleTypes = Enum.GetValues<VehicleCategoryType>().ToDictionary(x => x.ToString(), x => x);
        request.CategoryPricings.ForEach(async pricingDto =>
        {
            try
            {
                if(!vehicleTypes.TryGetValue(pricingDto.VehicleCategory, out var vehicleCategoryType))
                {
                    vehicleCategoryPricingErrors.Add(new Error("VC-Not-Found", $"Vehicle category {pricingDto.VehicleCategory} not found"));
                    return;
                }
                var existingCategoryPricing = await schoolManagementDirectory.SchoolCategoryPricingQueriesRepository.GetBySchoolAndCategory(
                    existingSchool.Id,
                    vehicleCategoryType);
                if(existingCategoryPricing.IsError)
                {
                    vehicleCategoryPricingErrors.AddRange(existingCategoryPricing.Errors);
                }
                else
                {
                    if(existingCategoryPricing is not null)
                    {
                        existingCategoryPricing.Value!.FullPrice=pricingDto.FullPrice;
                        existingCategoryPricing.Value!.Installments=pricingDto.Installments;
                        existingCategoryPricing.Value!.InstallmentPrice=pricingDto.InstallmentPrice;
                    }
                }
            }
            catch(Exception e)
            {
                vehicleCategoryPricingErrors.Add(new Error("VC-Pricing-Update", e.Message));
            }
        });

        return vehicleCategoryPricingErrors.Count!=0
            ? Result<School>.WithErrors([.. vehicleCategoryPricingErrors])
            : existingSchool;
    }

    private async Task<Result<School>> GetSchool(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var existingSchool = await schoolManagementDirectory.SchoolQueriesRepository.GetByIdAsync(request.SchoolId);

        if(!existingSchool.IsError&&existingSchool.Value is null)
        {
            return Result<School>.Failure(new Error("Not Found", "School not found"));
        }

        return !existingSchool.IsError
            ? existingSchool!
            : Result<School>.Failure([.. existingSchool.Errors, new Error("Not-Found", "Unable to retrieve school")]);
    }

    private async Task<Result<ApplicationUser>> GetOwner(FlexContext context)
    {
        var request = context.Get<UpdateSchoolCommand>();
        var school = context.Get<School>();
        var getUserById = new GetUserByIdQuery(request.RequestingUserId);
        var user = await sender.Send(getUserById);
        if(user.IsError)
        {
            return Result<ApplicationUser>.Failure([.. user.Errors, new Error("Owner-Not-Found", "Unable to retrieve school owner")]);
        }
        if(user.Value is null)
        {
            return Result<ApplicationUser>.Failure(new Error("Owner-Not-Found", "Owner not found"));
        }
        if(user.Value.Role.Name==ApplicationRole.IronMan.ToString())
        {
            return user;
        }

        if(user.Value.Id!=school.OwnerId)
        {
            return Result<ApplicationUser>.Failure(new Error("Owner-Mismatch", "The requesting user is not the owner of the school"));
        }
        return user;
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
            .Then(UpdateSlogan)
            .Then(UpdateStatistics)
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
            if((await schoolManagementDirectory.SchoolQueriesRepository.SchoolExists(request.LegalName)).Value)
                return Result<UpdateSchoolCommand>.Failure(new Error("Create-School", "Company name already exists"));

            return Result<UpdateSchoolCommand>.Success(request);
        }

        Result<UpdateSchoolCommand> UpdateImage(FlexContext localContext)
        {
            var request = localContext.Get<UpdateSchoolCommand>();
            var existingImage = existingSchool.Icon;
            //TODO: Check if image already exists, if so, update it, to be done after the whole image integration service

            return request;
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
                            .Then(_ => schoolManagementDirectory.VehicleQueriesRepository.GetByIdAsync((int)x))
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
                        .Then(_ => schoolManagementDirectory.CertificateQueriesRepository.GetByIdAsync((int)certificateType))
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
        Result<Statistics> UpdateStatistics(FlexContext context)
        {
            var request = context.Get<UpdateSchoolCommand>();
            var existingSchool = context.Get<School>();
            if(request.Statistics is null)
                return Result<Statistics>.Success(existingSchool.SchoolStatistics);
            existingSchool.SchoolStatistics.NumberOfStudents=request.Statistics.NumberOfStudents;

            return existingSchool.SchoolStatistics;
        }

        Result<School> UpdateSlogan(FlexContext context)
        {
            var request = context.Get<UpdateSchoolCommand>();
            var existingSchool = context.Get<School>();

            if(request.Slogan is not null)
            {
                existingSchool.ChangeSlogan((Slogan)request.Slogan);
            }

            return existingSchool;
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