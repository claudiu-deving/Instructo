using Domain.Shared;
namespace Domain.ValueObjects;

//City
public readonly record struct City
{
    public string Value { get; }
    private City(string value) =>
        Value=value;
    public static City Empty => new(string.Empty);
    public static Result<City> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<City>.Failure([new Error("City cannot be empty", value)]);
        if(value.Length>100)
            return Result<City>.Failure([new Error("City cannot be longer than 100 characters", value)]);
        return new City(value);
    }
    public static City Wrap(string value) => new(value);
    public static implicit operator string(City value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
