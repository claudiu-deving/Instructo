using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.ValueObjects;

using Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Configurations;

internal class SchoolsConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CompanyName);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name);
        builder.Property(x => x.CompanyName);
        builder.Property(x => x.Slug);
        builder.Property(x => x.Description);
        builder.Property(x => x.Slogan);
        builder.Property(x => x.Email);
        builder.Property(x => x.PhoneNumber).HasConversion(new PhoneNumberConverter());
        builder.Property(x => x.BussinessHours).HasConversion(new BussinessHoursConverter());
        builder.Property(x => x.PhoneNumbersGroups).HasConversion(new PhoneNumberConvertersGroupConverter());
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.VehicleCategories)
          .WithMany(c => c.Schools)
          .UsingEntity<SchoolCategory>(
              j => j.HasOne(sc => sc.VehicleCategory)
                    .WithMany()
                    .HasForeignKey(sc => sc.VehicleCategoryId),
              j => j.HasOne(sc => sc.School)
                    .WithMany()
                    .HasForeignKey(sc => sc.SchoolId),
              j =>
              {
                  j.HasKey(sc => new { sc.SchoolId, sc.VehicleCategoryId });
                  j.ToTable("SchoolCategories");
              });

        builder.HasMany(x => x.Certificates)
        .WithMany(c => c.Schools)
        .UsingEntity<SchoolCertificate>(
            j => j.HasOne(sc => sc.Certificate)
                  .WithMany()
                  .HasForeignKey(sc => sc.CertificateId),
            j => j.HasOne(sc => sc.School)
                  .WithMany()
                  .HasForeignKey(sc => sc.SchoolId),
            j =>
            {
                j.HasKey(sc => new { sc.SchoolId, sc.CertificateId });
                j.ToTable("SchoolCertificates");
            });

        builder.HasMany(s => s.WebsiteLinks)
            .WithMany(w => w.Schools)
            .UsingEntity(j => j.ToTable("SchoolWebsiteLinks"));
        builder.Navigation(s => s.WebsiteLinks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }


}
public class SchoolIdConverter(ConverterMappingHints? mappingHints = null) :
        ValueConverter<SchoolId, Guid>(x => x.Id, x => SchoolId.CreateNew(x), mappingHints);

public class SchoolNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<SchoolName, string>(x => x.Value, x => SchoolName.Wrap(x), mappingHints);


public class VehicleCategoryIdConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<VehicleCategoryType, int>(x => (int)x, x => (VehicleCategoryType)x, mappingHints);