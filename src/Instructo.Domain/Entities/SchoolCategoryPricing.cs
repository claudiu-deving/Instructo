using Domain.Common;
using Domain.Dtos;
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
    public Transmission? Transmission { get; set; }
    public int TransmissionId { get; set; }

    public SchoolCategoryPricingDto ToDto()
    {
        return new SchoolCategoryPricingDto(
            Category.Name,
            FullPrice,
            InstallmentPrice,
            Installments,
            Transmission?.Name??"Manual"
        );
    }
}
