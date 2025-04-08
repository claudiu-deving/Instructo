using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class EmailConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<Email, string>(x => x.Value, x => Email.Wrap(x), mappingHints);

