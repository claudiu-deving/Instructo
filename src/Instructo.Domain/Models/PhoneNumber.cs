using Domain.Shared;
namespace Domain.Models;

//Phone Number
public class PhoneNumber
{
    public PhoneNumber() { }
    public string? Name { get; set; }
    public string Value { get; set; } = string.Empty;
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
