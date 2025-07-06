using Domain.Shared;
namespace Domain.ValueObjects;

//Password
public readonly record struct Password
{
    public string Value { get; }
    private Password(string value) =>
        Value=value;
    public static Password Empty => new(string.Empty);
    public static Result<Password> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Password>.Failure([new Error("Password cannot be empty", value)]);
        if(value.Length>100)
            return Result<Password>.Failure([new Error("Password cannot be longer than 100 characters", value)]);
        return new Password(value);
    }
    public static Password Wrap(string value) => new(value);
    public static implicit operator string(Password value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
