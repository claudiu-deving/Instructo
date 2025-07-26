namespace Domain.Dtos;

public readonly record struct TeamDto(
    List<InstructorDto> Instructors
);