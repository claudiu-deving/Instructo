using Instructo.Domain.Dtos;
using Instructo.Domain.Entities;

namespace Instructo.Domain.Mappers;

public static class VehicleCategoryMapper
{
    public static VehicleCategoryDto ToDto(this VehicleCategory vehicleCategory) =>
        new VehicleCategoryDto(vehicleCategory.Name, vehicleCategory.Description);

}
