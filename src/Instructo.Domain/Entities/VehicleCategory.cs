using Domain.Common;
using Domain.Entities.SchoolEntities;
using Domain.Enums;

namespace Domain.Entities;

public class VehicleCategory : BaseEntity<VehicleCategoryType>
{
    private VehicleCategory(VehicleCategoryType id, string name, string description)
    {
        Id=id;
        Name=name;
        Description=description;
    }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<School> Schools { get; private set; } = [];
    public static VehicleCategory Create(VehicleCategoryType id, string description)
    {
        return new VehicleCategory(id, id.ToString(), description);
    }

    private VehicleCategory() { }
}
