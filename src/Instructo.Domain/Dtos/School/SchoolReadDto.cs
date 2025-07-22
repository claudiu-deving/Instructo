using Domain.Dtos.Image;

namespace Domain.Dtos.School;
/// <summary>
/// The data required to show a list of Schools
/// </summary>
public readonly record struct SchoolReadDto(
    Guid Id,
    string Name,
    string CompanyName,
    string Email,
    string PhoneNumber,
    string Slug,
    string CountyId,
    string CityName,
    string Slogan,
    string Description,
    ImageReadDto IconData
  ) : ISchoolReadDto;

public interface ISchoolReadDto
{
    string CompanyName { get; }
    string Name { get; }
    string Email { get; }
    string PhoneNumber { get; }
}