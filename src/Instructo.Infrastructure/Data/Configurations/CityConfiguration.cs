using System.Reflection;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations;
internal static class CityConfiguration
{
    public static void ConfigureCities(this ModelBuilder modelBuilder)
    {
        var cities = GenerateCities();
        modelBuilder.Entity<City>().HasData(cities);
    }
    private static List<City> GenerateCities()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Infrastructure.Data.Hardcoded.orase.csv";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var csvContent = reader.ReadToEnd();
        return CsvDataReader.ReadCitiesFromCsv(csvContent);
    }

}
