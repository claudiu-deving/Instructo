using Domain.Entities.SchoolEntities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;
internal class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
{
    public void Configure(EntityTypeBuilder<InstructorProfile> builder)
    {
        builder.HasOne(i => i.Team)
               .WithMany(t => t.Instructors)
               .HasForeignKey(i => i.TeamId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(i => i.ProfileImage)
               .WithMany()
               .HasForeignKey(i => i.ProfileImageId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(i => i.VehicleCategories)
               .WithMany(c => c.Instructors)
               .UsingEntity(j => j.ToTable("InstructorVehicleCategories"));
    }
}
