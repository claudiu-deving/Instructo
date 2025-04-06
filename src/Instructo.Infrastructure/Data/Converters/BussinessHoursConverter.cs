using Instructo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Instructo.Infrastructure.Data.Converters;

public class BussinessHoursConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<BussinessHours, string>(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<BussinessHours>(x), mappingHints);