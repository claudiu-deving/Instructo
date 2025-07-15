using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class SchoolNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<SchoolName, string>(x => x.Value, x => SchoolName.Wrap(x), mappingHints);
