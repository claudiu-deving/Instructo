using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Common;
using Instructo.Domain.Entities;
using Instructo.Domain.Shared;

namespace Instructo.Domain.ValueObjects;

public sealed class StudentId : ValueObject
{
    private StudentId(int id)
    {
        Id=id;
    }
    public int Id { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Id];
    }

    public static implicit operator int(StudentId studentId) => studentId.Id;

    public static StudentId Create(int id) => new(id);
}

