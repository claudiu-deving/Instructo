namespace Instructo.Domain.ValueObjects;

public sealed class WebsiteLinkId : ValueObject
{
    private WebsiteLinkId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(WebsiteLinkId studentId) => studentId.Id;

    public static WebsiteLinkId Create(int id) => new(id);
}
