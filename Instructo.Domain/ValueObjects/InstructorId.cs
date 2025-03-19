using Instructo.Domain.Common;

namespace Instructo.Domain.ValueObjects;

public sealed class InstructorId : ValueObject
{
    private InstructorId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(InstructorId studentId) => studentId.Id;

    public static InstructorId Create(int id) => new(id);
}
