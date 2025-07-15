using Domain.Entities;
using Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations;
internal static class CategoryConfiguration
{
    public static void ConfigureVehicleCategories(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.ToTable("VehicleCategories");
            entity.HasKey(e => e.Id);
            // Store the enum as an integer in the database
            entity.Property(e => e.Id)
                .HasConversion(x => (int)x, x => x);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);
        });
        SeedVehicleCategories(modelBuilder);
    }

    private static void SeedVehicleCategories(ModelBuilder modelBuilder)
    {
        var vehicleCategories = new List<VehicleCategory>
        {
            VehicleCategory.Create(
                VehicleCategoryType.AM,
                "Mopeds"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.A1,
                "Motorcycles with maximum 125cm³ cylinder capacity, maximum power of 11kW, and power-to-weight ratio not exceeding 0.1kW/kg; Motor tricycles with maximum power of 15kW"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.A2,
                "Motorcycles with maximum power of 35kW, power-to-weight ratio not exceeding 0.2kW/kg, and not derived from a vehicle with more than twice its power"
            ),
            VehicleCategory.Create(VehicleCategoryType.A,
                "Motorcycles with or without sidecar and motor tricycles with power over 15kW"),
            VehicleCategory.Create(VehicleCategoryType.B1,
                "Quadricycles with unladen mass not exceeding 400kg (550kg for goods transport vehicles), excluding the mass of batteries for electric vehicles, equipped with internal combustion engine not exceeding 15kW net maximum power or electric motor not exceeding 15kW continuous rated power"),
            VehicleCategory.Create(VehicleCategoryType.B,
                "Vehicles with maximum authorized mass not exceeding 3,500kg and with no more than 8 seats in addition to the driver's seat; Vehicle-trailer combinations where the trailer's maximum authorized mass doesn't exceed 750kg; Vehicle-trailer combinations not exceeding 4,250kg total, where the trailer's maximum authorized mass exceeds 750kg"),
            VehicleCategory.Create(VehicleCategoryType.BE,
                "Vehicle-trailer combinations exceeding 4,250kg total, comprising a category B vehicle and a trailer or semi-trailer with maximum authorized mass not exceeding 3,500kg"
            ),
            VehicleCategory.Create(VehicleCategoryType.C1,
                "Vehicles other than those in categories D or D1, with maximum authorized mass exceeding 3,500kg but not exceeding 7,500kg, designed to carry maximum 8 passengers in addition to the driver. These vehicles may be coupled with a trailer not exceeding 750kg maximum authorized mass"
            ),
            VehicleCategory.Create(VehicleCategoryType.C1E,
                "Vehicle-trailer combinations comprising a C1 vehicle and a trailer or semi-trailer with maximum authorized mass exceeding 750kg, provided the total doesn't exceed 12,000kg; Combinations where the towing vehicle is category B and the trailer or semi-trailer has a maximum authorized mass exceeding 3,500kg, provided the total doesn't exceed 12,000kg"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.C,
                "Vehicles other than those in categories D or D1, with maximum authorized mass exceeding 3,500kg, designed to carry maximum 8 passengers in addition to the driver; Combinations comprising a category C vehicle and a trailer with maximum authorized mass not exceeding 750kg"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.CE,
                "Vehicle-trailer combinations comprising a category C vehicle and a trailer or semi-trailer with maximum authorized mass exceeding 750kg"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.D1,
                "Vehicles designed to carry maximum 16 passengers in addition to the driver, with maximum length not exceeding 8m; Combinations comprising a D1 vehicle and a trailer with maximum authorized mass not exceeding 750kg"
            ),
            VehicleCategory.Create(
                VehicleCategoryType.D1E,
                "Vehicle-trailer combinations comprising a D1 vehicle and a trailer with maximum authorized mass exceeding 750kg. The trailer must not be designed to carry passengers"
            ),
            VehicleCategory.Create(VehicleCategoryType.D,
                "Vehicles designed to carry more than 8 passengers in addition to the driver. These vehicles may be coupled with a trailer not exceeding 750kg maximum authorized mass"
            ),
            VehicleCategory.Create(VehicleCategoryType.DE,
                "Vehicle-trailer combinations comprising a category D vehicle and a trailer with maximum authorized mass exceeding 750kg. The trailer must not be designed to carry passengers"
            ),
            VehicleCategory.Create(VehicleCategoryType.Tr,
                "Agricultural or forestry tractors"),
            VehicleCategory.Create(VehicleCategoryType.Tb,
                "Trolleybus"),
            VehicleCategory.Create(VehicleCategoryType.Tv,
                "Tram")
        };

        modelBuilder.Entity<VehicleCategory>().HasData(vehicleCategories);
    }
}
