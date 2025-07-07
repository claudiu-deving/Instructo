using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

// Note: This converter is only needed if you want to store Address as a string representation
// For proper spatial data storage, configure Address entity to use Point directly without converter
internal sealed class AddressConverter(ConverterMappingHints? mappingHints = null) :
ValueConverter<Address, string>(
    x => $"{x.Street}|{x.Longitude:F8}|{x.Latitude:F8}", 
    x => ParseAddressFromString(x), 
    mappingHints)
{
    private static Address ParseAddressFromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Address.CreateWithoutValidation("", 0, 0);
        
        var parts = value.Split('|');
        if (parts.Length != 3)
            return Address.CreateWithoutValidation("", 0, 0);
        
        var street = parts[0];
        if (!double.TryParse(parts[1], out var longitude))
            longitude = 0;
        if (!double.TryParse(parts[2], out var latitude))
            latitude = 0;
        
        return Address.CreateWithoutValidation(street, latitude, longitude);
    }
}