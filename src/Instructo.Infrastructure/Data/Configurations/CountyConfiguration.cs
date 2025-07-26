using System.Reflection;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations;
internal static class CountyConfiguration
{
    public static void ConfigureCounties(this ModelBuilder modelBuilder)
    {
        var counties = GenerateCounties();
        modelBuilder.Entity<County>().HasData(counties);
    }
    private static List<County> GenerateCounties()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Infrastructure.Data.Hardcoded.counties.csv";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ??throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");
        using var reader = new StreamReader(stream);
        var csvContent = reader.ReadToEnd();
        return CsvDataReader.ReadCountiesFromCsv(csvContent);
    }
}
