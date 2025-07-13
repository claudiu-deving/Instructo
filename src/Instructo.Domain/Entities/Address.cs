using Domain.Common;

using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class Address : IEntity
{
    // Romanian geographical boundaries
    private const double MIN_LATITUDE = 43.6;
    private const double MAX_LATITUDE = 48.3;
    private const double MIN_LONGITUDE = 20.3;
    private const double MAX_LONGITUDE = 29.7;
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

    public Address(string street, Point coordinate, string? comment = null)
    {
        Street=street??throw new ArgumentNullException(nameof(street));
        Coordinate=coordinate??throw new ArgumentNullException(nameof(coordinate));
        Comment=comment??string.Empty;
    }

    private Address()
    {
        // EF Core constructor
    }

    public int Id { get; init; }
    public string Street { get; } = string.Empty;
    public Point? Coordinate { get; }

    public string? Comment { get; }

    public static Address Empty => new("", _geometryFactory.CreatePoint(new Coordinate(0, 0)));

    public static Address Create(string street, string latitude, string longitude, string? comment = null)
    {
        if(string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));

        if(!double.TryParse(latitude, out var lat))
            throw new ArgumentException("Invalid latitude format", nameof(latitude));
        if(!double.TryParse(longitude, out var lon))
            throw new ArgumentException("Invalid longitude format", nameof(longitude));

        if(lat<MIN_LATITUDE||lat>MAX_LATITUDE)
            throw new ArgumentOutOfRangeException(nameof(latitude),
                $"Latitude must be between {MIN_LATITUDE} and {MAX_LATITUDE} for Romania");

        if(lon<MIN_LONGITUDE||lon>MAX_LONGITUDE)
            throw new ArgumentOutOfRangeException(nameof(longitude),
                $"Longitude must be between {MIN_LONGITUDE} and {MAX_LONGITUDE} for Romania");

        var point = _geometryFactory.CreatePoint(new Coordinate(lon, lat));

        return new Address(street, point, comment);
    }


    public static Address Create(string street, double latitude, double longitude, string? comment = null)
    {
        if(string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));

        if(latitude<MIN_LATITUDE||latitude>MAX_LATITUDE)
            throw new ArgumentOutOfRangeException(nameof(latitude),
                $"Latitude must be between {MIN_LATITUDE} and {MAX_LATITUDE} for Romania");

        if(longitude<MIN_LONGITUDE||longitude>MAX_LONGITUDE)
            throw new ArgumentOutOfRangeException(nameof(longitude),
                $"Longitude must be between {MIN_LONGITUDE} and {MAX_LONGITUDE} for Romania");

        var point = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        return new Address(street, point, comment);
    }

    public static Address CreateWithoutValidation(string street, double latitude, double longitude,
        string? comment = null)
    {
        var point = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        return new Address(street, point, comment);
    }

    public static Address CreateForSeeding(int id, string street, double latitude, double longitude,
        string? comment = null)
    {
        var point = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        return new Address(street, point, comment) { Id=id };
    }


    public override string ToString()
    {
        return $"{Street} ({Coordinate?.X:F6}, {Coordinate?.Y:F6}) {Comment}";
    }

    public override bool Equals(object? obj)
    {
        if(obj is not Address other)
            return false;
        if(Coordinate is null&&other.Coordinate is null)
        {
            return true;
        }
        if(Coordinate is null||other.Coordinate is null)
        {
            return false;
        }
        return Street==other.Street&&Coordinate.Equals(other.Coordinate);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Coordinate);
    }
}