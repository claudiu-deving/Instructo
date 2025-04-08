using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class UrlConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<Url, string>(x => x.Value, x => Url.Wrap(x), mappingHints);