namespace Instructo.Domain.Dtos;

public readonly record struct ChangePasswordDto
{
    public string Email { get; init; }
    public string CurrentPassword { get; init; }
    public string NewPassword { get; init; }
}