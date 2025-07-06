using Domain.Shared;
namespace Domain.ValueObjects;

public readonly record struct Url
{
    public string Value { get; }
    private Url(string value) =>
        Value=value;

    public static Url Empty => new(string.Empty);

    public static Result<Url> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Url>.Failure([new Error("Url cannot be empty", value)]);
        if(value.Length>2000)
            return Result<Url>.Failure([new Error("Url cannot be longer than 2000 characters", value)]);
        return new Url(value);
    }
    public static Url Wrap(string value) => new(value);

    public static implicit operator string(Url value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
