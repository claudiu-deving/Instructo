using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

public class CompanyNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<LegalName, string>(x => x.Value, x => LegalName.Wrap(x), mappingHints);
