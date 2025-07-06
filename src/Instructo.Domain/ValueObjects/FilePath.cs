using Domain.Shared;
namespace Domain.ValueObjects;

//FilePath
public readonly record struct FilePath
{
    public string Value { get; }
    private FilePath(string value) =>
        Value=value;
    public static FilePath Empty => new(string.Empty);
    public static Result<FilePath> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<FilePath>.Failure([new Error("File path cannot be empty", value)]);
        if(value.Length>100)
            return Result<FilePath>.Failure([new Error("File path cannot be longer than 100 characters", value)]);
        return new FilePath(value);
    }
    public static FilePath Wrap(string value) => new(value);
    public static implicit operator string(FilePath value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
