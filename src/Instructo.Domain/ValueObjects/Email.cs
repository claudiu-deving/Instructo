using Domain.Shared;
namespace Domain.ValueObjects;

//Email Domain Type
public readonly record struct Email
{
    public string Value { get; }
    private Email(string value) =>
        Value=value;
    public static Email Empty => new(string.Empty);
    public static Result<Email> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure([new Error("Email cannot be empty", value)]);
        if(value.Length>100)
            return Result<Email>.Failure([new Error("Email cannot be longer than 100 characters", value)]);
        return new Email(value);
    }
    public static Email Wrap(string value) => new(value);
    public static implicit operator string(Email value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
