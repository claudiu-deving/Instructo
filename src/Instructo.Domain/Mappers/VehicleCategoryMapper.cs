using Domain.Dtos;
using Domain.Entities;

namespace Domain.Mappers;

public static class VehicleCategoryMapper
{
    public static VehicleCategoryDto ToDto(this VehicleCategory vehicleCategory) =>
        new VehicleCategoryDto(vehicleCategory.Name, vehicleCategory.Description);

}
