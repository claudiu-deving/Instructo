using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class WebsiteLinkIdConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<WebsiteLinkId, int>(x => x.Id, x => WebsiteLinkId.Create(x), mappingHints);