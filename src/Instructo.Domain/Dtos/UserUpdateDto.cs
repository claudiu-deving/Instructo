namespace Instructo.Domain.Dtos;

public record UserUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
