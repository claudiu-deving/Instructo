using Instructo.Domain.Dtos;
using Instructo.Domain.Entities;

namespace Instructo.Domain.Mappers;

public static class ArrCertificationMapper
{
    public static ArrCertificationDto ToDto(this ArrCertificate vehicleCategory) =>
        new ArrCertificationDto(vehicleCategory.Id.ToString(), vehicleCategory.Name);
}
