using Domain.Shared;

namespace Domain.ValueObjects;

public readonly record struct Slogan(string Value)
{
    public static Slogan Empty => new(string.Empty);
    public static Result<Slogan> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Slogan>.Failure([new Error("Slogan cannot be empty", value)]);
        if(value.Length>200)
            return Result<Slogan>.Failure([new Error("Slogan cannot be longer than 200 characters", value)]);
        return new Slogan(value);
    }
}
