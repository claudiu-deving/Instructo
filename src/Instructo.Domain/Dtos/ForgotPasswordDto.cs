namespace Instructo.Domain.Dtos;

public readonly record struct ForgotPasswordDto
{
    public string Email { get; init; }
}