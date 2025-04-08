using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class WebsiteLinkNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<WebsiteLinkName, string>(x => x.Value, x => WebsiteLinkName.Wrap(x), mappingHints);
