using Domain.Dtos.Image;
using Domain.Dtos.School;
using Domain.Entities;

namespace Domain.Mappers;

public static class SchoolMappers
{
    public static SchoolDetailReadDto ToDetailedReadDto(this School school)
    {
        if(school.City is null||school.County is null)
        {
            throw new ArgumentNullException($"{school} doesn't have City or County attached");
        }

        return new SchoolDetailReadDto(
            school.Id,
            school.Name,
            school.CompanyName,
            school.Email,
            school.PhoneNumber,
            school.Slug,
            school.County.Code,
            school.City.Name,
            school.Slogan,
            school.Description,
            school.PhoneNumbersGroups.Select(x => x.ToDto()),
            school.Icon?.ToReadDto()??ImageReadDto.Empty,
           [.. school.WebsiteLinks.Select(x => x.ToReadDto())],
            school.BussinessHours,
           [.. school.VehicleCategories.Select(x => x.ToDto())],
           [.. school.Certificates.Select(x => x.ToDto())],
           school.SchoolStatistics.ToDto(),
           [.. school.CategoryPricings.Select(x => x.ToDto())],
           school.Team.ToDto(),
           [.. school.Locations.Select(x => x.ToDto(Enums.AddressType.LessonStart).ValueOrThrow())]
        );
    }
}