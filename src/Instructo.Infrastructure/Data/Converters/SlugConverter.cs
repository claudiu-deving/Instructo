using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class SlugConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<Slug, string>(x => x.Value, x => Slug.Wrap(x), mappingHints);
