using System.Collections.Generic;
using System.Numerics;

using Instructo.Domain.Common;
using Instructo.Domain.Enums;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities;

public sealed class School : BaseAuditableEntity<SchoolId>
{
    private readonly List<Instructor> _instructors = [];

    public School(SchoolId id) : base(id)
    {
    }

    public IReadOnlyList<Instructor> Instructors => _instructors;
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

    private Enrollment(EnrollmentId id, Student student, School school, Instructor instructor, int numberOfSessionsTodo) : base(id)
    {
        Student=student;
        School=school;
        Instructor=instructor;
        NumberOfSessionsToDo=numberOfSessionsTodo;
    }

    public Student Student { get; }
    public School School { get; }
    public Instructor Instructor { get; }

    public Result<Session> RegisterSession(Session session)
    {
        _sessions.Add(session);
        NumberOfSessionsDone++;
        return session;
    }

    public static Result<Enrollment> Create(EnrollmentId id, Student student, School school, Instructor instructor, int numberOfSessionsTodo)
    {
        if(!school.Instructors.Contains(instructor))
        {
            return new Error("Instructor.NotInSchool", "The instructor must be part of the school");
        }

        return new Enrollment(id, student, school, instructor, numberOfSessionsTodo);
    }
}
