using Domain.Shared;
namespace Domain.ValueObjects;

public readonly record struct ContentType
{
    public string Value { get; }
    private ContentType(string value) =>
        Value=value;

    public static ContentType Empty => new(string.Empty);

    public static Result<ContentType> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<ContentType>.Failure([new Error("Content type cannot be empty", value)]);
        if(value.Length>100)
            return Result<ContentType>.Failure([new Error("Content type cannot be longer than 100 characters", value)]);
        return new ContentType(value);
    }
    public static ContentType Wrap(string value) => new(value);

    public static implicit operator string(ContentType value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
