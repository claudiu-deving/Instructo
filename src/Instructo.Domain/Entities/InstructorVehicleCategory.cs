using Domain.Common;
using Domain.Entities.SchoolEntities;

namespace Domain.Entities;

public class InstructorVehicleCategory : IEntity
{
    public Guid InstructorId { get; set; }
    public int VehicleCategoryId { get; set; }
    public InstructorProfile Instructor { get; set; } = null!;
    public VehicleCategory VehicleCategory { get; set; } = null!;
}