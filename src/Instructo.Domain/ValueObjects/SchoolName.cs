using Domain.Shared;
namespace Domain.ValueObjects;

public readonly record struct SchoolName
{
    public string Value { get; }
    private SchoolName(string value) =>
        Value=value;

    public static SchoolName Empty => new(string.Empty);

    public static Result<SchoolName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<SchoolName>.Failure([new Error("School name cannot be empty", value)]);
        if(value.Length>100)
            return Result<SchoolName>.Failure([new Error("School name cannot be longer than 100 characters", value)]);
        return new SchoolName(value);
    }
    public static SchoolName Wrap(string value) => new(value);

    public static implicit operator string(SchoolName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
