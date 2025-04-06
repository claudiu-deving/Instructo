using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class UrlConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<Url, string>(x => x.Value, x => Url.Wrap(x), mappingHints);