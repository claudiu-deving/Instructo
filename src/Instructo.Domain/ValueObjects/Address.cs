using Domain.Shared;
namespace Domain.ValueObjects;

//Address
public readonly record struct Address
{
    public string Value { get; }
    private Address(string value) =>
        Value=value;
    public static Address Empty => new(string.Empty);
    public static Result<Address> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Address>.Failure([new Error("Address cannot be empty", value)]);
        if(value.Length>100)
            return Result<Address>.Failure([new Error("Address cannot be longer than 100 characters", value)]);
        return new Address(value);
    }
    public static Address Wrap(string value) => new(value);
    public static implicit operator string(Address value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
