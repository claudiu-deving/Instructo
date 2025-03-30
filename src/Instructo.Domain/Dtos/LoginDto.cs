namespace Instructo.Domain.Dtos;

public readonly record struct LoginDto
{
    public string Email { get; init; }
    public string Password { get; init; }
}
