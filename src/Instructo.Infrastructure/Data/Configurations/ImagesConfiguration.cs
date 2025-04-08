using Domain.Entities;

using Infrastructure.Data.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;
internal class ImagesConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(new ImageIdConverter());
        builder.Property(x => x.FileName).HasConversion(new FileNameConverter());
        builder.Property(x => x.ContentType).HasConversion(new ContentTypeConverter());
        builder.Property(x => x.Url).HasConversion(new UrlConverter());
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.ContentType).IsRequired();
    }
}