namespace Instructo.Domain.ValueObjects;

public sealed class ImageId : ValueObject
{
    private ImageId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }
    public static implicit operator int(ImageId studentId) => studentId.Id;
    public static ImageId Create(int id) => new(id);
}
