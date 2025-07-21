using Domain.Shared;

using static Domain.ValueObjects.ResultHelperExtensions;
namespace Domain.ValueObjects;

public readonly record struct LegalName
{
    public string Value { get; }
    private LegalName(string value) =>
        Value=value;

    public static LegalName Empty => new(string.Empty);

    public static Result<LegalName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Failure<LegalName>(value, "Company name cannot be empty");
        value=value.Trim();
        if(value.Length>200)
            return Failure<LegalName>(value, "Company name cannot be longer than 200 characters");
        if(value.FirstOrDefault().ToString().Equals(value.FirstOrDefault().ToString().ToLower()))
            return Failure<LegalName>(value, "Company name must start with a capital letter");

        return new LegalName(value);
    }
    public static LegalName Wrap(string value) => new(value);
    public static implicit operator string(LegalName value) => value.Value;

    public override string ToString()
    {
        return Value;
    }
}
