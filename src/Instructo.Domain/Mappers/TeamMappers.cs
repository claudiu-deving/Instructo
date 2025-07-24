using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;

namespace Domain.Mappers;

public static class TeamMappers
{
    public static TeamDto ToDto(this Team? team)
    {
        if(team==null)
        {
            return new TeamDto([]);
        }
        return new TeamDto(
            [.. team.Instructors.Select(i => i.ToDto())]
        );
    }

    public static InstructorDto ToDto(this InstructorProfile instructor)
    {
        var currentYear = DateTime.Now.Year;
        var age = currentYear-instructor.BirthYear;

        return new InstructorDto(
            instructor.Id,
            instructor.FirstName,
            instructor.LastName,
            age,
            instructor.YearsExperience,
           instructor.VehicleCategories.Select(vc => vc.ToDto()).ToList(),
           instructor.Gender,
            instructor.Specialization,
            instructor.Description,
             instructor.Email,
           instructor.Phone,
            instructor.ProfileImage?.FileName,
            instructor.ProfileImage?.Url,
            instructor.ProfileImage?.ContentType,
            instructor.ProfileImage?.Description
        );
    }
}