using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Models;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Commands.CreateSchool;

public partial record CreateSchoolCommand
{
    public static Result<CreateSchoolCommand> Create(CreateSchoolCommandDto createSchoolCommandDto)
    {
        List<Error> errors = [];
        CreateSchoolCommand createSchoolCommand = new();
        LegalName.Create(createSchoolCommandDto.LegalName)
                 .OnSuccess(value => createSchoolCommand.LegalName=value)
                 .OnError(errors.AddRange);
        createSchoolCommand.Slogan=new Slogan(createSchoolCommandDto.Slogan??"");
        createSchoolCommand.Description=new Description(createSchoolCommandDto.Description);

        SchoolName.Create(createSchoolCommandDto.Name)
                  .OnSuccess(value => createSchoolCommand.Name=value)
                  .OnError(errors.AddRange);

        Email.Create(createSchoolCommandDto.OwnerEmail)
            .OnSuccess(value => createSchoolCommand.OwnerEmail=value)
            .OnError(errors.AddRange);

        Email.Create(createSchoolCommandDto.SchoolEmail)
            .OnSuccess(value => createSchoolCommand.SchoolEmail=value)
            .OnError(errors.AddRange);

        CityDto.Create(createSchoolCommandDto.City)
            .OnSuccess(value => createSchoolCommand.City=value)
            .OnError(errors.AddRange);

        AddressDto.Create(createSchoolCommandDto.Address, createSchoolCommandDto.X, createSchoolCommandDto.Y)
            .OnSuccess(value => createSchoolCommand.Address=value)
            .OnError(errors.AddRange);


        ValidatePhoneNumbers(createSchoolCommandDto)
                  .OnSuccess(phoneNumberData =>
                  {
                      createSchoolCommand.PhoneNumber=phoneNumberData.phoneNumber;
                      createSchoolCommand.PhoneNumbersGroups= [.. phoneNumberData.numberGroups];
                  })
                  .OnError(errors.AddRange);

        FilePath.Create(createSchoolCommandDto.ImagePath)
            .OnSuccess(value => createSchoolCommand.ImagePath=value)
            .OnError(errors.AddRange);

        ContentType.Create(createSchoolCommandDto.ImageContentType)
            .OnSuccess(value => createSchoolCommand.ImageContentType=value)
            .OnError(errors.AddRange);


        createSchoolCommand.WebsiteLink=createSchoolCommandDto.WebsiteLink;

        createSchoolCommand.SocialMediaLinks=createSchoolCommandDto.SocialMediaLinks;

        BussinessHours.Create(createSchoolCommandDto.BussinessHours)
            .OnSuccess(value => createSchoolCommand.BussinessHours=value)
            .OnError(errors.AddRange);

        if(createSchoolCommandDto.VechiclesCategories.Count==0)
        {
            errors.Add(new Error("VC-Empty", "There must be at least 1 category"));
        }
        List<VehicleCategoryType> categories = [];
        createSchoolCommandDto.VechiclesCategories.ForEach(category =>
        {
            if(TryParseCategory(category, out VehicleCategoryType vehicleCategory))
            {
                categories.Add(vehicleCategory);
            }
            else
            {
                errors.Add(new Error("VC-Parse", $"{category} is not a valid vehicle category"));
            }
        });
        createSchoolCommand.VehicleCategories=categories;


        List<ARRCertificateType> certificates = [];
        var parsedEnum = Enum.GetValues<ARRCertificateType>().Select(x => x.ToString());
        createSchoolCommandDto.ArrCertifications.ForEach(certificateInput =>
        {
            if(!parsedEnum.Contains(certificateInput))
            {
                errors.Add(new Error("ARRCertificate-Parse", $"Unable to parse {certificateInput}"));
            }
            else
            {
                certificates.Add(Enum.Parse<ARRCertificateType>(certificateInput));
            }
        });
        createSchoolCommand.Certificates=certificates;


        createSchoolCommand.Statistics=new Statistics()
        {
            NumberOfStudents=createSchoolCommandDto.NumberOfStudents
        };

        createSchoolCommand.CategoryPricings=createSchoolCommandDto.CategoryPricings;

        if(errors.Count!=0)
        {
            return Result<CreateSchoolCommand>.Failure([.. errors]);
        }
        else
        {
            return Result<CreateSchoolCommand>.Success(createSchoolCommand);
        }
    }

    private static Result<(IEnumerable<PhoneNumbersGroup> numberGroups, PhoneNumber phoneNumber)> ValidatePhoneNumbers(CreateSchoolCommandDto createSchoolCommandDto)
    {
        List<Error> errors = [];
        List<PhoneNumbersGroup> phoneNumberGroups = [];
        PhoneNumber? mainPhoneNumber = null;
        if(createSchoolCommandDto.PhoneNumberGroups.Count!=0)
        {
            foreach(PhoneNumberGroupDto group in createSchoolCommandDto.PhoneNumberGroups)
            {
                PhoneNumbersGroup phoneNumberGroup = new PhoneNumbersGroup()
                {
                    Name=group.Name
                };
                foreach(PhoneNumberDto phoneNumber in group.PhoneNumbers)
                {
                    PhoneNumber.Create(phoneNumber.Value, phoneNumber.Name)
                               .OnError(errors.AddRange)
                               .OnSuccess(phoneNumberGroup.PhoneNumbers.Add);
                }
                if(errors.Count==0)
                {
                    phoneNumberGroups.Add(phoneNumberGroup);
                }
                else
                {
                    break;
                }
            }
            if(phoneNumberGroups.Count==1&&!MainPhoneNumberExists(createSchoolCommandDto))
            {
                if(phoneNumberGroups[0].PhoneNumbers.Count==1)
                {
                    mainPhoneNumber=phoneNumberGroups[0].PhoneNumbers[0];
                    phoneNumberGroups.Clear();
                }
            }
            if(phoneNumberGroups.Count>=1&&phoneNumberGroups[0].PhoneNumbers.Count==1)
            {
                mainPhoneNumber=phoneNumberGroups[0].PhoneNumbers[0];
                phoneNumberGroups[0].PhoneNumbers.RemoveAt(0);
                phoneNumberGroups.RemoveAt(0);
            }
            if(phoneNumberGroups.Count>=1&&phoneNumberGroups[0].PhoneNumbers.Count>1)
            {
                mainPhoneNumber=phoneNumberGroups[0].PhoneNumbers[0];
                phoneNumberGroups[0].PhoneNumbers.RemoveAt(0);
            }

            if(MainPhoneNumberExists(createSchoolCommandDto))
            {
                var phoneNumberCreationRequest = PhoneNumber.Create(createSchoolCommandDto.PhoneNumber!)
                   .OnError(errors.AddRange)
                   .OnSuccess(phoneNum => mainPhoneNumber=phoneNum);
            }
        }
        else
        {
            if(createSchoolCommandDto.PhoneNumber is null)
            {
                errors.Add(new Error("Phone-Number-Missing", "No phone number groups and no phone number"));
            }
            else
            {
                var phoneNumberCreationRequest = PhoneNumber.Create(createSchoolCommandDto.PhoneNumber)
                    .OnError(errors.AddRange)
                    .OnSuccess(phoneNum => mainPhoneNumber=phoneNum);
            }
        }
        if(errors.Count!=0)
        {
            return Result<(IEnumerable<PhoneNumbersGroup>, PhoneNumber)>.WithErrors([.. errors]);
        }
        if(mainPhoneNumber is null)
        {
            mainPhoneNumber=phoneNumberGroups[0].PhoneNumbers[0];
        }


        return Result<(IEnumerable<PhoneNumbersGroup>, PhoneNumber)>.Success((phoneNumberGroups, mainPhoneNumber));
    }

    private static bool MainPhoneNumberExists(CreateSchoolCommandDto createSchoolCommandDto)
    {
        return createSchoolCommandDto.PhoneNumber is not null&&!string.IsNullOrEmpty(createSchoolCommandDto.PhoneNumber);
    }

    /// <summary>
    /// Attempts to parse a string representation of a vehicle certificate without throwing an exception.
    /// </summary>
    /// <param name="categoryCode">The vehicle certificate code as a string.</param>
    /// <param name="category">When this method returns, contains the parsed VehicleCategoryType if successful, or the default value if parsing failed.</param>
    /// <returns>true if the parsing was successful; otherwise, false.</returns>
    public static bool TryParseCategory(string categoryCode, out VehicleCategoryType category)
    {
        category=default;

        if(string.IsNullOrWhiteSpace(categoryCode))
        {
            return false;
        }

        // Normalize the input by trimming and converting to uppercase
        categoryCode=categoryCode.Trim().ToUpperInvariant();

        switch(categoryCode)
        {
            case "AM":
                category=VehicleCategoryType.AM;
                return true;
            case "A1":
                category=VehicleCategoryType.A1;
                return true;
            case "A2":
                category=VehicleCategoryType.A2;
                return true;
            case "A":
                category=VehicleCategoryType.A;
                return true;
            case "B1":
                category=VehicleCategoryType.B1;
                return true;
            case "B":
                category=VehicleCategoryType.B;
                return true;
            case "BE":
                category=VehicleCategoryType.BE;
                return true;
            case "C1":
                category=VehicleCategoryType.C1;
                return true;
            case "C1E":
                category=VehicleCategoryType.C1E;
                return true;
            case "C":
                category=VehicleCategoryType.C;
                return true;
            case "CE":
                category=VehicleCategoryType.CE;
                return true;
            case "D1":
                category=VehicleCategoryType.D1;
                return true;
            case "D1E":
                category=VehicleCategoryType.D1E;
                return true;
            case "D":
                category=VehicleCategoryType.D;
                return true;
            case "DE":
                category=VehicleCategoryType.DE;
                return true;
            case "TR":
                category=VehicleCategoryType.Tr;
                return true;
            case "TB":
                category=VehicleCategoryType.Tb;
                return true;
            case "TV":
                category=VehicleCategoryType.Tv;
                return true;
            default:
                return false;
        }
    }
}
