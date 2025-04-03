using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class UrlConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<Url, string>(x => x.Value, x => new Url(x), mappingHints);