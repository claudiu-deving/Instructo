using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

internal sealed class WebsiteLinkIdConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<WebsiteLinkId, Guid>(x => x.Id, x => WebsiteLinkId.CreateNew(), mappingHints);