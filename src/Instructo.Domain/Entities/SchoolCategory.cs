using Domain.Common;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;
public class SchoolCategory : IEntity
{
    public Guid SchoolId { get; set; }
    public int VehicleCategoryId { get; set; }

    public School School { get; set; } = null!;
    public VehicleCategory VehicleCategory { get; set; } = null!;
}
