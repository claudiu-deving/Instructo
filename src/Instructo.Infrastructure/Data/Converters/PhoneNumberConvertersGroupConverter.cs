using Domain.Entities.SchoolEntities;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Infrastructure.Data.Converters;

internal sealed class PhoneNumberConvertersGroupConverter(ConverterMappingHints? mappingHints = null) :
    ValueConverter<List<PhoneNumbersGroup>, string>(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<PhoneNumbersGroup>>(x), mappingHints);