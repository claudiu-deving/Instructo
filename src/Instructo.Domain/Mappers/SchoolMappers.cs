using Domain.Dtos.Image;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;

namespace Domain.Mappers;

public static class SchoolMappers
{
    public static SchoolDetailReadDto ToReadDto(this School school)
    {
        if(school.City is null&&school.County is null)
        {
            throw new ArgumentNullException($"{school} doesn't have City or County attached");
        }
        return new SchoolDetailReadDto(
            school.Id,
            school.Name.Value,
            school.CompanyName.Value,
            school.Email,
            school.PhoneNumber,
            school.Slug,
            school.County.Code,
            school.City.Name,
            school.Slogan.Value,
            school.Description.Value,
            school.Address.Street,
            school.Address.Coordinate?.X??0,
            school.Address.Coordinate?.Y??0,
            school.PhoneNumbersGroups.Select(x => x.ToDto()),
            school.Icon?.ToReadDto()??ImageReadDto.Empty,
           [.. school.WebsiteLinks.Select(x => x.ToReadDto())],
           [.. school.BussinessHours.BussinessHoursEntries],
           [.. school.VehicleCategories.Select(x => x.ToDto())],
           [.. school.Certificates.Select(x => x.ToDto())]
        );
    }
}