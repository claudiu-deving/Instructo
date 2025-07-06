using Domain.Shared;
namespace Domain.ValueObjects;

public readonly record struct WebsiteLinkName
{
    public string Value { get; }
    private WebsiteLinkName(string value) =>
        Value=value;

    public static WebsiteLinkName Empty => new(string.Empty);

    public static Result<WebsiteLinkName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<WebsiteLinkName>.Failure([new Error("Website link name cannot be empty", value)]);
        if(value.Length>100)
            return Result<WebsiteLinkName>.Failure([new Error("Website link name cannot be longer than 100 characters", value)]);
        return new WebsiteLinkName(value);
    }

    public static WebsiteLinkName Wrap(string value) => new(value);

    public static implicit operator string(WebsiteLinkName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
