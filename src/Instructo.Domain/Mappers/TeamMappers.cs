using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;

namespace Domain.Mappers;

public static class TeamMappers
{
    public static TeamDto ToDto(this Team? team)
    {
        if(team==null)
        {
            return new TeamDto(0, new Dictionary<string, int>(), []);
        }
        return new TeamDto(
            team.TotalInstructors,
            team.InstructorsByGender,
            [.. team.Instructors.Select(i => i.ToDto())]
        );
    }

    public static InstructorDto ToDto(this InstructorProfile instructor)
    {
        var currentYear = DateTime.Now.Year;
        var age = currentYear-instructor.BirthYear;

        return new InstructorDto(
            instructor.FirstName,
            instructor.LastName,
            age,
            instructor.YearsExperience,
            instructor.VehicleCategories.Select(vc => vc.Name).ToList(),
            instructor.Specialization,
            instructor.Description,
            new ContactDto(instructor.Phone, instructor.Email),
            instructor.ProfileImage?.Url
        );
    }
}