namespace Domain.Dtos.School;

public readonly record struct InstructorDto(
    string FirstName,
    string LastName,
    int Age,
    int YearsExperience,
    List<VehicleCategoryDto> Categories,
    string Gender,
    string? Specialization,
    string? Description,
    string? Email,
    string? PhoneNumber,
    string? ProfileImageName,
    string? ProfileImageUrl,
    string? ProfileImageContentType,
    string? ProfileImageDescription
);
