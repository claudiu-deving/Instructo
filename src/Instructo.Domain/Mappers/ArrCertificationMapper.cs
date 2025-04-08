using Domain.Dtos;
using Domain.Entities;

namespace Domain.Mappers;

public static class ArrCertificationMapper
{
    public static ArrCertificationDto ToDto(this ArrCertificate vehicleCategory) =>
        new ArrCertificationDto(vehicleCategory.Id.ToString(), vehicleCategory.Name);
}
