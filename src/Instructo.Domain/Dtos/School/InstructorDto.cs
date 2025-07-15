namespace Domain.Dtos.School;

public readonly record struct InstructorDto(
    string FirstName,
    string LastName,
    int Age,
    int YearsExperience,
    List<string> Categories,
    string Specialization,
    string Description,
    ContactDto Contact,
    string? ProfileImage
);
