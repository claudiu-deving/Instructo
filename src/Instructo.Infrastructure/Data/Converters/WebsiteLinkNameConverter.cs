using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class WebsiteLinkNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<WebsiteLinkName, string>(x => x.Value, x => WebsiteLinkName.Wrap(x), mappingHints);
