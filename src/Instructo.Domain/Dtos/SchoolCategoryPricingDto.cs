namespace Domain.Dtos;
public readonly record struct SchoolCategoryPricingDto
(
     string VehicleCategory,
     decimal FullPrice,
     decimal InstallmentPrice,
     int Installments,
     string Transmission
);
