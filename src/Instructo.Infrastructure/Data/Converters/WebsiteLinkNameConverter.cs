using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class WebsiteLinkNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<WebsiteLinkName, string>(x => x.Name, x => new WebsiteLinkName(x), mappingHints);
