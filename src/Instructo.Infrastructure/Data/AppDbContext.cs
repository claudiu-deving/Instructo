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

        modelBuilder.ConfigureIdentity();

        modelBuilder.ConfigureArrCertificates();

        modelBuilder.ConfigureVehicleCategories();

        modelBuilder.ConfigureCounties();

        modelBuilder.ConfigureCities();

        modelBuilder.Entity<SchoolDetailReadDto>(entity =>
        {
            entity.HasNoKey().ToView("SchoolDetails");

            entity.Property(x => x.ArrCertificates).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<List<ArrCertificationDto>>(x)??new List<ArrCertificationDto>())
                                .Metadata.SetValueComparer(new ValueComparer<List<ArrCertificationDto>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));
            entity.Property(x => x.VehicleCategories).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<List<VehicleCategoryDto>>(x)??new List<VehicleCategoryDto>())
                                .Metadata.SetValueComparer(new ValueComparer<List<VehicleCategoryDto>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));
            entity.Property(x => x.PhoneNumbersGroups).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<IEnumerable<PhoneNumberGroupDto>>(x)??new List<PhoneNumberGroupDto>())
                                .Metadata.SetValueComparer(new ValueComparer<IEnumerable<PhoneNumberGroupDto>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));

            entity.Property(x => x.Links).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<WebsiteLinkRead[]>(x))
                                .Metadata.SetValueComparer(new ValueComparer<WebsiteLinkRead[]>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToArray()));
            entity.Property(x => x.BussinessHours).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<BussinessHours>(x));
            entity.Property(x => x.IconData).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<ImageReadDto>(x));
            entity.Property(x => x.ExtraLocations).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<List<AddressDto>>(x)??new List<AddressDto>())
                                .Metadata.SetValueComparer(new ValueComparer<List<AddressDto>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));
            entity.Property(x => x.CategoryPricings).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<List<SchoolCategoryPricingDto>>(x)??new List<SchoolCategoryPricingDto>())
                                .Metadata.SetValueComparer(new ValueComparer<List<SchoolCategoryPricingDto>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));
            entity.Property(x => x.Team).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<TeamDto>(x));
            entity.Property(x => x.SchoolStatistics).HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<SchoolStatisticsDto>(x));
        });
    }



}
