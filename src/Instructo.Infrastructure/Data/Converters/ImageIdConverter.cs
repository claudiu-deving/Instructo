using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class ImageIdConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<ImageId, Guid>(x => x.Id, x => ImageId.CreateNew(), mappingHints);