using Application.Schools.Commands.CreateSchool;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Commands.UpdateSchool;

public partial record UpdateSchoolCommand
{
    public static Result<UpdateSchoolCommand> Create(UpdateSchoolCommandDto updateSchoolCommandDto, SchoolId schoolId,
        Guid requestingUserId)
    {
        List<Error> errors = [];
        UpdateSchoolCommand updateSchoolCommand = new();
        updateSchoolCommand.RequestingUserId = requestingUserId;
        updateSchoolCommand.SchoolId = schoolId;
        if (updateSchoolCommandDto.LegalName is not null)
            Domain.ValueObjects.LegalName.Create(updateSchoolCommandDto.LegalName)
                .OnSuccess(value => updateSchoolCommand.LegalName = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.Name is not null)
            SchoolName.Create(updateSchoolCommandDto.Name)
                .OnSuccess(value => updateSchoolCommand.Name = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.OwnerEmail is not null)
            Email.Create(updateSchoolCommandDto.OwnerEmail)
                .OnSuccess(value => updateSchoolCommand.OwnerEmail = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.SchoolEmail is not null)
            Email.Create(updateSchoolCommandDto.SchoolEmail)
                .OnSuccess(value => updateSchoolCommand.SchoolEmail = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.OwnerFirstName is not null)
            Domain.ValueObjects.Name.Create(updateSchoolCommandDto.OwnerFirstName)
                .OnSuccess(value => updateSchoolCommand.OwnerFirstName = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.OwnerLastName is not null)
            Domain.ValueObjects.Name.Create(updateSchoolCommandDto.OwnerLastName)
                .OnSuccess(value => updateSchoolCommand.OwnerLastName = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.City is not null)
            Domain.ValueObjects.City.Create(updateSchoolCommandDto.City)
                .OnSuccess(value => updateSchoolCommand.City = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.Address is not null)
            Domain.ValueObjects.Address.Create(updateSchoolCommandDto.Address)
                .OnSuccess(value => updateSchoolCommand.Address = value)
                .OnError(errors.AddRange);

        if (updateSchoolCommandDto.PhoneNumber is not null)
            ValidatePhoneNumbers(updateSchoolCommandDto)
                .OnSuccess(phoneNumberData =>
                {
                    updateSchoolCommand.PhoneNumbersGroups = [.. phoneNumberData.numberGroups];
                })
                .OnError(errors.AddRange);

        if (updateSchoolCommandDto.ImagePath is not null)
            FilePath.Create(updateSchoolCommandDto.ImagePath)
                .OnSuccess(value => updateSchoolCommand.ImagePath = value)
                .OnError(errors.AddRange);
        if (updateSchoolCommandDto.ImageContentType is not null)
            ContentType.Create(updateSchoolCommandDto.ImageContentType)
                .OnSuccess(value => updateSchoolCommand.ImageContentType = value)
                .OnError(errors.AddRange);


        updateSchoolCommand.WebsiteLink = updateSchoolCommandDto.WebsiteLink;

        updateSchoolCommand.SocialMediaLinks = updateSchoolCommandDto.SocialMediaLinks;
        if (updateSchoolCommandDto.BusinessHours is not null)
            BussinessHours.Create(updateSchoolCommandDto.BusinessHours)
                .OnSuccess(value => updateSchoolCommand.BussinessHours = value)
                .OnError(errors.AddRange);

        if (updateSchoolCommandDto.VehiclesCategories is not null)
        {
            if (updateSchoolCommandDto.VehiclesCategories.Count == 0)
            {
                errors.Add(new Error("VC-Empty", "There must be at least 1 category"));
            }
            else
            {
                List<VehicleCategoryType> categories = [];
                updateSchoolCommandDto.VehiclesCategories!.ForEach(category =>
                {
                    if (CreateSchoolCommand.TryParseCategory(category, out var vehicleCategory))
                        categories.Add(vehicleCategory);
                    else
                        errors.Add(new Error("VC-Parse", $"{category} is not a valid vehicle category"));
                });
                updateSchoolCommand.VehicleCategories = categories;
            }
        }


        if (updateSchoolCommandDto.ArrCertifications is not null)
        {
            List<ARRCertificateType> certificates = [];
            var parsedEnum = Enum.GetValues<ARRCertificateType>().Select(x => x.ToString());
            updateSchoolCommandDto.ArrCertifications.ForEach(certificateInput =>
            {
                if (!parsedEnum.Contains(certificateInput))
                    errors.Add(new Error("ARRCertificate-Parse", $"Unable to parse {certificateInput}"));
                else
                    certificates.Add(Enum.Parse<ARRCertificateType>(certificateInput));
            });
            updateSchoolCommand.Certificates = certificates;
        }

        if (errors.Count != 0) return Result<UpdateSchoolCommand>.Failure([.. errors]);


        return Result<UpdateSchoolCommand>.Success(updateSchoolCommand);
    }

    private static Result<(IEnumerable<PhoneNumbersGroup> numberGroups, PhoneNumber phoneNumber)>
        ValidatePhoneNumbers(UpdateSchoolCommandDto updateSchoolCommandDto)
    {
        List<Error> errors = [];
        List<PhoneNumbersGroup> phoneNumberGroups = [];
        PhoneNumber? mainPhoneNumber = null;
        if (updateSchoolCommandDto.PhoneNumberGroups is not null && updateSchoolCommandDto.PhoneNumberGroups.Count != 0)
        {
            foreach (var group in updateSchoolCommandDto.PhoneNumberGroups)
            {
                var phoneNumberGroup = new PhoneNumbersGroup
                {
                    Name = group.Name
                };
                foreach (var phoneNumber in group.PhoneNumbers)
                    PhoneNumber.Create(phoneNumber.Value, phoneNumber.Name)
                        .OnError(errors.AddRange)
                        .OnSuccess(phoneNum => phoneNumberGroup.PhoneNumbers.Add(phoneNum));
                if (errors.Count == 0)
                    phoneNumberGroups.Add(phoneNumberGroup);
                else
                    break;
            }
        }
        else
        {
            if (updateSchoolCommandDto.PhoneNumber is not null)
                PhoneNumber.Create(updateSchoolCommandDto.PhoneNumber)
                    .OnError(errors.AddRange)
                    .OnSuccess(phoneNum => mainPhoneNumber = phoneNum);
        }

        if (errors.Count != 0) return Result<(IEnumerable<PhoneNumbersGroup>, PhoneNumber)>.WithErrors([.. errors]);
        mainPhoneNumber ??= phoneNumberGroups[0].PhoneNumbers[0];

        return Result<(IEnumerable<PhoneNumbersGroup>, PhoneNumber)>.Success((phoneNumberGroups, mainPhoneNumber));
    }

    private static bool MainPhoneNumberExists(UpdateSchoolCommandDto createSchoolCommandDto)
    {
        return createSchoolCommandDto.PhoneNumber is not null &&
               !string.IsNullOrEmpty(createSchoolCommandDto.PhoneNumber);
    }
}