using Instructo.Domain.Common;

namespace Instructo.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    private UserId(Guid id)
    {
        Id=id;
    }
    public Guid Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator Guid(UserId studentId) => studentId.Id;

    public static UserId Create(Guid id) => new(id);
}