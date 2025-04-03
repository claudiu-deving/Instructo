namespace Instructo.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    private UserId(string id)
    {
        Id=id;
    }
    public string Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator string(UserId userId) => userId.Id;

    public static UserId Create(string id) => new(id);
}