using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class ContentTypeConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<ContentType, string>(x => x.Name, x => new ContentType(x), mappingHints);