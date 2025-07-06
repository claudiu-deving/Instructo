using Domain.Shared;
namespace Domain.ValueObjects;

public readonly record struct FileName
{
    public string Value { get; }
    private FileName(string value) =>
        Value=value;

    public static FileName Empty => new(string.Empty);

    public static Result<FileName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<FileName>.Failure([new Error("File name cannot be empty", value)]);
        if(value.Length>100)
            return Result<FileName>.Failure([new Error("File name cannot be longer than 100 characters", value)]);
        return new FileName(value);
    }

    public static FileName Wrap(string value) => new(value);
    public static implicit operator string(FileName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
