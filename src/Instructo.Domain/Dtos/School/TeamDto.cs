namespace Domain.Dtos.School;

public readonly record struct TeamDto(
    int TotalInstructors,
    Dictionary<string, int> InstructorsByGender,
    List<InstructorDto> Instructors
);