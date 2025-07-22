using System;
using System.Text.RegularExpressions;

using Domain.Dtos;
using Domain.Dtos.Image;
using Domain.Dtos.Link;
using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

using Infrastructure.Data.Configurations;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<School> Schools { get; set; }
    public DbSet<WebsiteLink> WebsiteLinks { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<ArrCertificate> CertificateTypes { get; set; }
    public DbSet<VehicleCategory> Categories { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<County> Counties { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<SchoolCategory> SchoolCategories { get; set; }
    public DbSet<SchoolCertificate> SchoolCertificates { get; set; }
    public DbSet<SchoolCategoryPricing> SchoolCategoryPricings { get; set; }
    public DbSet<InstructorProfile> Instructors { get; set; }
    public DbSet<Transmission> Transmissions { get; set; }
    public DbSet<Team> Teams { get; set; }

    public DbSet<SchoolDetailReadDto> SchoolDetails { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(x => x.UseNetTopologySuite());
        optionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SchoolConfiguration());
        modelBuilder.ApplyConfiguration(new WebsiteLinksConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new SchoolCategoryPricingConfiguration());
        modelBuilder.ApplyConfiguration(new InstructorProfileConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new SchoolDetailConfiguration());
        modelBuilder.ConfigureIdentity();

        modelBuilder.ConfigureArrCertificates();

        modelBuilder.ConfigureVehicleCategories();

        modelBuilder.ConfigureCounties();

        modelBuilder.ConfigureCities();

        modelBuilder.Entity<Transmission>().HasData(
            new Transmission { Id=1, Name="Manual" },
            new Transmission { Id=2, Name="Automatic" }
        );

    }



}
