using Domain.Entities;
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

        builder.HasMany(x => x.VehicleCategories)
         .WithMany(c => c.Instructors)
         .UsingEntity<InstructorVehicleCategory>(
             j => j.HasOne(sc => sc.VehicleCategory)
                   .WithMany()
                   .HasForeignKey(sc => sc.VehicleCategoryId),
             j => j.HasOne(sc => sc.Instructor)
                   .WithMany()
                   .HasForeignKey(sc => sc.InstructorId),
             j =>
             {
                 j.HasKey(sc => new { sc.InstructorId, sc.VehicleCategoryId });
                 j.ToTable("InstructorVehicleCategories");
             });
    }
}
