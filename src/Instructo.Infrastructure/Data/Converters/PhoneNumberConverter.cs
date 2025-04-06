using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class PhoneNumberConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<PhoneNumber, string>(x => x.Value, x => PhoneNumber.Wrap(x), mappingHints);
