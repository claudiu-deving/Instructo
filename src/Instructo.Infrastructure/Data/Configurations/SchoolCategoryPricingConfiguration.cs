using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;
internal class SchoolCategoryPricingConfiguration : IEntityTypeConfiguration<SchoolCategoryPricing>
{
    public void Configure(EntityTypeBuilder<SchoolCategoryPricing> builder)
    {
        builder.HasKey(scp => new { scp.SchoolId, scp.VehicleCategoryId, scp.TransmissionId });
        builder.Property(x => x.FullPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.InstallmentPrice).HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.Transmission)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.School)
            .WithMany(s => s.CategoryPricings)
            .HasForeignKey(scp => scp.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
