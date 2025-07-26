using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class VehicleCategory : IEntity
{
    private VehicleCategory(int id, string name, string description)
    {
        Id=id;
        Name=name;
        Description=description;
    }

    private VehicleCategory()
    {
    }

    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual ICollection<School> Schools { get; private set; } = [];

    public virtual ICollection<InstructorProfile> Instructors { get; private set; } = [];

    public static VehicleCategory Create(VehicleCategoryType id, string description)
    {
        return new VehicleCategory((int)id, id.ToString(), description);
    }
}