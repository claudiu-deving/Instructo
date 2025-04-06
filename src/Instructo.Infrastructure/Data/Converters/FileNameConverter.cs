using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class FileNameConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<FileName, string>(x => x.Value, x => FileName.Wrap(x), mappingHints);