using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class ContentTypeConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<ContentType, string>(x => x.Value, x => ContentType.Wrap(x), mappingHints);