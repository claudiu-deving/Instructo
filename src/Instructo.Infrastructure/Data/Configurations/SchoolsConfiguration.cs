using Domain.Entities.SchoolEntities;
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
        builder.Property(x => x.Id).HasConversion(new SchoolIdConverter()).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasConversion(new SchoolNameConverter());
        builder.Property(x => x.CompanyName).HasConversion(new CompanyNameConverter());
        builder.Property(x => x.Email).HasConversion(new EmailConverter());
        builder.Property(x => x.PhoneNumber).HasConversion(new PhoneNumberConverter());
        builder.Property(x => x.BussinessHours).HasConversion(new BussinessHoursConverter());
        builder.Property(x => x.PhoneNumbersGroups).HasConversion(new PhoneNumberConvertersGroupConverter());
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.VehicleCategories)
            .WithMany(c => c.Schools)
            .UsingEntity(j => j.ToTable("SchoolCategories"));

        builder.HasMany(x => x.Certificates)
            .WithMany(c => c.Schools)
            .UsingEntity(j => j.ToTable("SchoolCertificates"));

        builder.HasMany(s => s.WebsiteLinks)
            .WithMany(w => w.Schools)
            .UsingEntity(j => j.ToTable("SchoolWebsiteLinks"));
        builder.Navigation(s => s.WebsiteLinks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private class SchoolIdConverter(ConverterMappingHints? mappingHints = null) :
        ValueConverter<SchoolId, Guid>(x => x.Id, x => SchoolId.CreateNew(), mappingHints);

    private class SchoolNameConverter(ConverterMappingHints? mappingHints = null) :
        ValueConverter<SchoolName, string>(x => x.Value, x => SchoolName.Wrap(x), mappingHints);
}