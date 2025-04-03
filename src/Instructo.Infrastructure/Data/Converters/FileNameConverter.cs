using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal class FileNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<FileName, string>(x => x.Name, x => new FileName(x), mappingHints);