namespace Instructo.Domain.Dtos.User;

public readonly record struct ForgotPasswordDto
{
    public string Email { get; init; }
}