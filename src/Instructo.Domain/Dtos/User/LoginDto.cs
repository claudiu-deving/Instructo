﻿namespace Domain.Dtos.User;

public readonly record struct LoginDto
{
    public string Email { get; init; }
    public string Password { get; init; }
}
