using Domain.Common;
using Domain.Dtos;
using Domain.Entities.SchoolEntities;
using Domain.Enums;

namespace Domain.Entities;

public class SchoolCategoryPricing : IEntity
{
    public Guid SchoolId { get; set; }
    public virtual School School { get; set; } = null!;
    public virtual VehicleCategory Category { get; set; } = null!;
    public int VehicleCategoryId { get; set; }
    public decimal FullPrice { get; set; }
    public decimal InstallmentPrice { get; set; }
    public int Installments { get; set; } = 5;
    public TransmissionType Transmission { get; set; } = TransmissionType.Manual;

    public SchoolCategoryPricingDto ToDto()
    {
        return new SchoolCategoryPricingDto(
            Category.Name,
            FullPrice,
            InstallmentPrice,
            Installments,
            Transmission.ToString()
        );
    }
}
