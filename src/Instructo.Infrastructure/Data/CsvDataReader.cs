using Domain.Entities;

using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Infrastructure.Data;

/// <summary>
/// A service class for reading location data.
/// </summary>
public class CsvDataReader
{

    /// <summary>
    /// Reads city data from a CSV string and converts it into a list of County objects.
    /// </summary>
    /// <param name="csvContent">A string containing the CSV data.</param>
    /// <returns>A list of County objects.</returns>
    public static List<County> ReadCountiesFromCsv(string csvContent)
    {
        var counties = new List<County>();

        // Use a StringReader to process the string content line by line
        using var reader = new StringReader(csvContent);

        // Read and discard the header line
        string? headerLine = reader.ReadLine();
        if(headerLine==null)
        {
            // Return an empty list if the content is empty
            return counties;
        }

        string? currentLine;
        int currentId = 1; // Start ID from 1

        // Loop through the rest of the lines in the CSV
        while((currentLine=reader.ReadLine())!=null)
        {
            if(string.IsNullOrWhiteSpace(currentLine))
            {
                continue; // Skip empty lines
            }

            var values = currentLine.Split(',');

            // Basic validation to ensure the line has at least 2 columns
            if(values.Length>=2)
            {
                var county = new County
                {
                    Id=currentId++,
                    Name=values[0].Trim(),
                    Code=values[1].Trim(),
                    Cities= [] // Initialize as an empty list
                };
                counties.Add(county);
            }
            else
            {
                // In a real-world application, you might want to log this error
                Console.WriteLine($"Skipping malformed line: {currentLine}");
            }
        }

        return counties;
    }
    private static Dictionary<string, int> _countyData = new Dictionary<string, int>()
    {
        ["AB"]=1,
        ["AR"]=2,
        ["AG"]=3,
        ["BC"]=4,
        ["BH"]=5,
        ["BN"]=6,
        ["BT"]=7,
        ["BV"]=8,
        ["BR"]=9,
        ["B"]=10,
        ["BZ"]=11,
        ["CS"]=12,
        ["CL"]=13,
        ["CJ"]=14,
        ["CT"]=15,
        ["CV"]=16,
        ["DB"]=17,
        ["DJ"]=18,
        ["GL"]=19,
        ["GR"]=20,
        ["GJ"]=21,
        ["HR"]=22,
        ["HD"]=23,
        ["IL"]=24,
        ["IS"]=25,
        ["IF"]=26,
        ["MM"]=27,
        ["MH"]=28,
        ["MS"]=29,
        ["NT"]=30,
        ["OT"]=31,
        ["PH"]=32,
        ["SM"]=33,
        ["SJ"]=34,
        ["SB"]=35,
        ["SV"]=36,
        ["TR"]=37,
        ["TM"]=38,
        ["TL"]=39,
        ["VS"]=40,
        ["VL"]=41,
        ["VN"]=42
    };
    /// <summary>
    /// Reads city data from a CSV string and converts it into a list of County objects.
    /// </summary>
    /// <param name="csvContent">A string containing the CSV data.</param>
    /// <returns>A list of County objects.</returns>
    public static List<City> ReadCitiesFromCsv(string csvContent)
    {
        var _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var counties = new List<City>();

        // Use a StringReader to process the string content line by line
        using var reader = new StringReader(csvContent);

        // Read and discard the header line
        string? headerLine = reader.ReadLine();
        if(headerLine==null)
        {
            // Return an empty list if the content is empty
            return counties;
        }

        string? currentLine;
        int currentId = 1; // Start ID from 1

        // Loop through the rest of the lines in the CSV
        while((currentLine=reader.ReadLine())!=null)
        {
            if(string.IsNullOrWhiteSpace(currentLine))
            {
                continue; // Skip empty lines
            }

            var values = currentLine.Split(',');

            // Basic validation to ensure the line has at least 2 columns
            if(values.Length>=2)
            {
                if(!double.TryParse(values[0], out double x))
                    continue;
                if(!double.TryParse(values[1], out double y))
                    continue;
                var city = new City
                {
                    Id=currentId++,
                    Name=values[2].Trim(),
                    CountyId=_countyData[values[4]],
                    Location=_geometryFactory.CreatePoint(new Coordinate(y, x))
                };
                counties.Add(city);
            }
            else
            {
                // In a real-world application, you might want to log this error
                Console.WriteLine($"Skipping malformed line: {currentLine}");
            }
        }

        Console.WriteLine($"Created: {counties.Count} cities");

        return counties;
    }
}