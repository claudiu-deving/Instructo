using Domain.Shared;

namespace Domain.ValueObjects;

public readonly record struct Description(string Value)
{
    public static Description Empty => new(string.Empty);
    public static Result<Description> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Description>.Failure([new Error("Description cannot be empty", value)]);
        if(value.Length>500)
            return Result<Description>.Failure([new Error("Description cannot be longer than 500 characters", value)]);
        return new Description(value);
    }
    public static Description Wrap(string value) => new(value);
    public static implicit operator string(Description value) => value.Value;
    public override string ToString() => Value;
}
