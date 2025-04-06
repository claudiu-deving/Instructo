using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instructo.Infrastructure.Data.Converters;

internal sealed class EmailConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<Email, string>(x => x.Value, x => Email.Wrap(x), mappingHints);

