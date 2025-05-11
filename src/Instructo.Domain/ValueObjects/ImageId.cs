namespace Domain.ValueObjects;

public readonly record struct ImageId(Guid Id)
{
    public ImageId() : this(Guid.NewGuid())
    {
    }

    public static implicit operator Guid(ImageId imageId)
    {
        return imageId.Id;
    }

    public static ImageId CreateNew()
    {
        return new ImageId();
    }

    public static ImageId Wrap(Guid value)
    {
        return new ImageId(value);
    }
}