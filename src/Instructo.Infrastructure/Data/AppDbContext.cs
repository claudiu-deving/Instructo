using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for your entity models
    public DbSet<School> Schools { get; set; }
    public DbSet<WebsiteLink> WebsiteLinks { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<ArrCertificate> CertificateTypes { get; set; }
    public DbSet<VehicleCategory> Categories { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new SchoolsConfiguration());
        modelBuilder.ApplyConfiguration(new WebsiteLinksConfiguration());
        modelBuilder.ApplyConfiguration(new ImagesConfiguration());

        modelBuilder.Entity<ApplicationUser>().ToTable("Users").HasIndex(u => u.Email).IsUnique();
        //modelBuilder.Entity<ApplicationUser>().HasOne(x => x.School).WithOne(s => s.Owner);
        modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        ConfigCertificates(modelBuilder);

        ConfigCategories(modelBuilder);
    }

    public override int SaveChanges()
    {
        SetToUnchanged();

        return base.SaveChanges();
    }

    /// <summary>
    ///     We retrieve these hard coded entities with Dapper, EF doesn't know about them
    /// </summary>
    private void SetToUnchanged()
    {
        foreach (var entry in ChangeTracker.Entries<VehicleCategory>()
                     .Where(e => e.State == EntityState.Added))
            entry.State = EntityState.Unchanged;

        foreach (var entry in ChangeTracker.Entries<ArrCertificate>()
                     .Where(e => e.State == EntityState.Added))
            entry.State = EntityState.Unchanged;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetToUnchanged();

        return base.SaveChangesAsync(cancellationToken);
    }


    private static void ConfigCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.ToTable("VehicleCategories");
            entity.HasKey(e => e.Id);
            // Store the enum as an integer in the database
            entity.Property(e => e.Id)
                .HasConversion(x => (int)x, x => (VehicleCategoryType)x);

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

    private static void ConfigCertificates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArrCertificate>(entity =>
        {
            entity.ToTable("ARRCertificates");
            entity.HasKey(e => e.Id);

            // Store the enum as an integer in the database
            entity.Property(e => e.Id)
                .HasConversion(x => (int)x, x => (ARRCertificateType)x);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);
        });
        SeedCertificateTypes(modelBuilder);
    }

    private static void SeedCertificateTypes(ModelBuilder modelBuilder)
    {
        var certificateTypes = new List<ArrCertificate>
        {
            ArrCertificate.Create(
                ARRCertificateType.FreightTransport,
                "Atestat pentru transport marfă",
                "Certificate for general goods transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.PassengerTransport,
                "Atestat pentru transport persoane",
                "Certificate for passenger transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DangerousGoodsTransport,
                "Atestat ADR",
                "Certificate for dangerous goods transportation (ADR)"
            ),
            ArrCertificate.Create(
                ARRCertificateType.OversizedTransport,
                "Atestat pentru transport agabaritic",
                "Certificate for oversized load transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.TaxiTransport,
                "Atestat pentru transport taxi",
                "Certificate for taxi transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.TransportManager,
                "Atestat pentru manager de transport (CPI)",
                "Certificate for transport managers (CPI)"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DrivingInstructor,
                "Atestat pentru instructor auto",
                "Certificate for driving instructors"
            ),
            ArrCertificate.Create(
                ARRCertificateType.RoadLegislationTeacher,
                "Atestat pentru profesor de legislație rutieră",
                "Certificate for road legislation teachers"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DangerousGoodsSafetyAdvisor,
                "Atestat pentru consilier de siguranță",
                "Certificate for safety advisors for the transport of dangerous goods"
            )
        };

        modelBuilder.Entity<ArrCertificate>().HasData(certificateTypes);
    }
}