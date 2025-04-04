using Instructo.Domain.Entities;
using Instructo.Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instructo.Infrastructure.Data.Configurations;
internal class WebsiteLinksConfiguration : IEntityTypeConfiguration<WebsiteLink>
{
    public void Configure(EntityTypeBuilder<WebsiteLink> builder)
    {
        builder.ToTable("WebsiteLinks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(new WebsiteLinkIdConverter());
        builder.Property(x => x.Url).HasConversion(new UrlConverter());
        builder.Property(x => x.Name).HasConversion(new WebsiteLinkNameConverter());
    }
}