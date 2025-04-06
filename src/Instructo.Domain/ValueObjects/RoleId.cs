namespace Instructo.Domain.ValueObjects;

public readonly record struct RoleId(Guid Id)
{
    public RoleId() : this(Guid.NewGuid()) { }
    public static implicit operator Guid(RoleId roleId) => roleId.Id;
    public static RoleId CreateNew() => new();
}