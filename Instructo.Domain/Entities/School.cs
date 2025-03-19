using System.Collections.Generic;
using System.Numerics;

using Instructo.Domain.Common;
using Instructo.Domain.Enums;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities;

public sealed class School : BaseAuditableEntity<SchoolId>
{
    public School(SchoolId id) : base(id)
    {
    }
}

public class User<T>(T id) : BaseAuditableEntity<T>(id)
{

}
public sealed class Owner<T>(T id) : User<T>(id)
{

}
public sealed class Instructor : User<InstructorId>
{
    public Instructor(InstructorId id) : base(id)
    {
    }
}

public sealed class Student : User<StudentId>
{
    public Student(StudentId id) : base(id)
    {
    }

}

public sealed class Session : BaseAuditableEntity<SessionId>
{
    public Session(SessionId id) : base(id)
    {
    }
}

public sealed class Vehicle : BaseAuditableEntity<VehicleId>
{
    public Vehicle(VehicleId id) : base(id)
    {
    }
}
public sealed class Enrollment : BaseAuditableEntity<EnrollmentId>
{

    public DateTime EnrollmentStartUtc { get; }
    public EnrollmentStatus Status { get; }
    public int NumberOfSessionsToDo { get; }
    public int NumberOfSessionsDone { get; private set; }
    public IReadOnlyList<Session> Sessions => _sessions;

    private readonly List<Session> _sessions = [];

    public Enrollment(EnrollmentId id, StudentId studentId, SchoolId schoolId) : base(id)
    {
    }

    public Student Student { get; }

    public Result<Session> RegisterSession(Session session)
    {
        _sessions.Add(session);
        NumberOfSessionsDone++;
        return session;
    }
}
