﻿using Domain.Shared;
namespace Domain.ValueObjects;

//Phone Number
public record PhoneNumber
{
    private PhoneNumber() { }
    public string? Name { get; private set; }
    public string Value { get; private set; } = string.Empty;
    private PhoneNumber(string value, string? name = null)
    {
        Value=value;
        Name=name;
    }

    public static PhoneNumber Empty => new(string.Empty);
    public static Result<PhoneNumber> Create(string value, string? name = null)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<PhoneNumber>.Failure([new Error("Phone number cannot be empty", value)]);
        if(value.Length>100)
            return Result<PhoneNumber>.Failure([new Error("Phone number cannot be longer than 100 characters", value)]);
        return new PhoneNumber(value, name);
    }
    public static PhoneNumber Wrap(string value) => new(value);
    public static implicit operator string(PhoneNumber value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
