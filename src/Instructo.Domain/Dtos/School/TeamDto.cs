namespace Domain.Dtos.School;

public readonly record struct TeamDto(
    List<InstructorDto> Instructors
);