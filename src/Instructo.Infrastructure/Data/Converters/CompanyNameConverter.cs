using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class CompanyNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<LegalName, string>(x => x.Value, x => LegalName.Wrap(x), mappingHints);
