using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

namespace Instructo.Application.Schools.Commands.CreateSchool;

public record CreateSchoolCommand(
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
    string ImageContentType) : ICommand<Result<SchoolReadDto>>;
