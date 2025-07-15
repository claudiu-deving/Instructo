using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class SchoolIdConverter(ConverterMappingHints? mappingHints = null) :
        ValueConverter<SchoolId, Guid>(x => x.Id, x => SchoolId.CreateNew(x), mappingHints);
