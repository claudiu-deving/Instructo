namespace Instructo.Domain.ValueObjects;


public readonly record struct SchoolId(Guid Id)
{
    public SchoolId() : this(Guid.NewGuid()) { }
    public static implicit operator Guid(SchoolId imageId) => imageId.Id;
    public static implicit operator SchoolId(Guid guid) =>CreateNew(guid);
    public static SchoolId CreateNew() => new();
    public static SchoolId CreateNew(Guid id) => new(id);
}
