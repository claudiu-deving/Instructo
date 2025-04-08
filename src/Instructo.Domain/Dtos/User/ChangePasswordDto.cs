namespace Domain.Dtos.User;

public readonly record struct ChangePasswordDto
{
    public string Email { get; init; }
    public string CurrentPassword { get; init; }
    public string NewPassword { get; init; }
}