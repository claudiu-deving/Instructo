namespace Instructo.Domain.ValueObjects;


public readonly record struct UserId(Guid Id)
{
    public UserId() : this(Guid.NewGuid()) { }
    public static implicit operator Guid(UserId userId) => userId.Id;
    public static UserId CreateNew() => new();
}
