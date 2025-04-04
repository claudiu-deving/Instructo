namespace Instructo.Application.Schools.Commands.CreateSchool;

public readonly record struct CreateSchoolCommandDto(
    string Name,
    string CompanyName,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName,
    string City,
    string Address,
    string PhoneNumber,
    string ImagePath,
    string ImageContentType);
