using Instructo.Domain.Common;

namespace Instructo.Domain.ValueObjects;

public sealed class VehicleId : ValueObject
{
    private VehicleId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(VehicleId studentId) => studentId.Id;

    public static VehicleId Create(int id) => new(id);
}
