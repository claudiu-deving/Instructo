using Instructo.Shared;

namespace Instructo.Domain.Entities;

public class SchoolEntity : BaseAuditableEntity
{
    public SchoolEntity(int id) : base()
    {
        Id=id;
    }

    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public required string Contact { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Website { get; set; }
    public required string Logo { get; set; }
    public virtual ApplicationUser? Owner { get; set; }
}

