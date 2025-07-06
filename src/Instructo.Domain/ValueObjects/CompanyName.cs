using Domain.Shared;
namespace Domain.ValueObjects;

//Company name
public readonly record struct CompanyName
{
    public string Value { get; }
    private CompanyName(string value) =>
        Value=value;
    public static CompanyName Empty => new(string.Empty);
    public static Result<CompanyName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<CompanyName>.Failure([new Error("Company name cannot be empty", value)]);
        if(value.Length>100)
            return Result<CompanyName>.Failure([new Error("Company name cannot be longer than 100 characters", value)]);
        return new CompanyName(value);
    }
    public static CompanyName Wrap(string value) => new(value);
    public static implicit operator string(CompanyName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}