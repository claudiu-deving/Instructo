namespace Instructo.Domain.ValueObjects;

public sealed class SessionId : ValueObject
{
    private SessionId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(SessionId studentId) => studentId.Id;

    public static SessionId Create(int id) => new(id);
}
