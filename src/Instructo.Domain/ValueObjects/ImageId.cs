namespace Domain.ValueObjects;

public readonly record struct ImageId(Guid Id)
{
    public ImageId() : this(Guid.NewGuid()) { }
    public static implicit operator Guid(ImageId imageId) => imageId.Id;
    public static ImageId CreateNew() => new();
}
