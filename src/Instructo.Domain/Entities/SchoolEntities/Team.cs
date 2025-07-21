using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SchoolEntities;

public class Team : BaseAuditableEntity<Guid>
{
    private readonly List<InstructorProfile> _instructors = [];

    private Team(Guid schoolId)
    {
        Id=Guid.NewGuid();
        SchoolId=schoolId;
    }

    private Team() { }

    [ForeignKey("School")]
    public Guid SchoolId { get; private set; }

    public virtual School School { get; private set; } = null!;
    public virtual IReadOnlyCollection<InstructorProfile> Instructors => _instructors.AsReadOnly();

    public void AddInstructor(InstructorProfile instructor)
    {
        if(_instructors.Contains(instructor))
            return;
        _instructors.Add(instructor);
    }

    public bool RemoveInstructor(InstructorProfile instructor)
    {
        if(!_instructors.Contains(instructor))
            return false;
        _instructors.Remove(instructor);
        return true;
    }

    public void ClearInstructors()
    {
        _instructors.Clear();
    }

    public static Team Create(Guid schoolId)
    {
        return new Team(schoolId);
    }
}