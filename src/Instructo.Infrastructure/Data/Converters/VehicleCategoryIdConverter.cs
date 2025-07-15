using Domain.Enums;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class VehicleCategoryIdConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<VehicleCategoryType, int>(x => (int)x, x => (VehicleCategoryType)x, mappingHints);