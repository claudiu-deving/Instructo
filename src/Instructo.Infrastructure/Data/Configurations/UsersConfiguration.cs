using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Instructo.Domain.Entities;

namespace Instructo.Infrastructure.Data.Configurations;

class SchoolsConfiguration : IEntityTypeConfiguration<SchoolEntity>
{
    public void Configure(EntityTypeBuilder<SchoolEntity> builder)
    {
        builder.ToTable("Schools");
        builder.HasKey(x => x.Id);
    }
}
