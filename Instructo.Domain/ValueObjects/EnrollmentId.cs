using Instructo.Domain.Common;

namespace Instructo.Domain.ValueObjects;

public sealed class EnrollmentId : ValueObject
{
    private EnrollmentId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(EnrollmentId studentId) => studentId.Id;

    public static EnrollmentId Create(int id) => new(id);
}

