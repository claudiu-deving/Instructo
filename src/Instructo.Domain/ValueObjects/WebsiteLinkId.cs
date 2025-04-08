namespace Domain.ValueObjects;

public readonly record struct WebsiteLinkId(Guid Id)
{
    public WebsiteLinkId() : this(Guid.NewGuid()) { }
    public static implicit operator Guid(WebsiteLinkId imageId) => imageId.Id;
    public static WebsiteLinkId CreateNew() => new();
}
