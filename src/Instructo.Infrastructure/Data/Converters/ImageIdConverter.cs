using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class ImageIdConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<ImageId, int>(x => x.Id, x => ImageId.Create(x), mappingHints);