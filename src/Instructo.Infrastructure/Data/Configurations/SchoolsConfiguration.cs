using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Instructo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Instructo.Domain.ValueObjects;

namespace Instructo.Infrastructure.Data.Configurations;

class SchoolsConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(new SchoolIdConverter());
        builder.Property(x => x.Name).HasConversion(new SchoolNameConverter());
        builder.Property(x => x.CompanyName).HasConversion(new CompanyNameConverter());
        builder.Property(x => x.Name).IsRequired();
    }
}

public class SchoolIdConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<SchoolId, int>(x => x.Id, x => SchoolId.Create(x), mappingHints);

public class SchoolNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<SchoolName, string>(x => x.Name, x => new SchoolName(x), mappingHints);

public class CompanyNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<CompanyName, string>(x => x.Name, x => new CompanyName(x), mappingHints);