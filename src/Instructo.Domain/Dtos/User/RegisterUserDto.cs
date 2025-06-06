﻿namespace Domain.Dtos.User;

public readonly record struct RegisterUserDto
{
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Password { get; init; }
    public string PhoneNumber { get; init; }
    public string Role { get; init; }
}
