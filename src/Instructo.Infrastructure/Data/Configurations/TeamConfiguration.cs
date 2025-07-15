using Domain.Entities.SchoolEntities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;
internal class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasMany(x => x.Instructors)
               .WithOne(x => x.Team)
               .HasForeignKey(x => x.TeamId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
