using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class ContentTypeConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<ContentType, string>(x => x.Value, x => ContentType.Wrap(x), mappingHints);