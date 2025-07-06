using Domain.Shared;
namespace Domain.ValueObjects;

//Person First name
public readonly record struct Name
{
    public string Value { get; }
    private Name(string value) =>
        Value=value;
    public static Name Empty => new(string.Empty);
    public static Result<Name> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Name>.Failure([new Error("Name cannot be empty", value)]);
        if(value.Length>100)
            return Result<Name>.Failure([new Error("Name cannot be longer than 100 characters", value)]);
        return new Name(value);
    }
    public static Name Wrap(string value) => new(value);
    public static implicit operator string(Name value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
