using Instructo.Domain.Dtos.Image;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Entities.SchoolEntities;

namespace Instructo.Domain.Mappers;

public static class SchoolMappers
{
    public static SchoolReadDto ToReadDto(this School school)
    {
        return new SchoolReadDto(

            school.Id,
            school.Name.Value,
            school.CompanyName.Value,
            school.Email,
            school.PhoneNumber,
            school.PhoneNumbersGroups.Select(x => x.ToDto()),
            school.Icon?.ToReadDto()??ImageReadDto.Empty,
           [.. school.WebsiteLinks.Select(x => x.ToReadDto())],
           [.. school.BussinessHours.BussinessHoursEntries],
           [.. school.VehicleCategories.Select(x => x.ToDto())],
           [.. school.Certificates.Select(x => x.ToDto())]
        );
    }
}