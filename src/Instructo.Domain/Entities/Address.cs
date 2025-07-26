using System.ComponentModel.DataAnnotations.Schema;

using Domain.Common;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Shared;
using Domain.ValueObjects;

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

    public Address(string street, Point coordinate, AddressType addressType, string? comment = null)
    {
        Street=street??throw new ArgumentNullException(nameof(street));
        Coordinate=coordinate??throw new ArgumentNullException(nameof(coordinate));
        Comment=comment??string.Empty;
        AddressType=addressType;
    }

    private Address()
    {
        // EF Core constructor
    }

    public int Id { get; init; }
    public string Street { get; private set; } = string.Empty;
    public Point? Coordinate { get; private set; }
    public string? Comment { get; private set; }

    public virtual School? School { get; private set; }

    public AddressType AddressType { get; private set; }

    public void ChangeAddressType(AddressType newAddressType)
    {
        if(newAddressType==AddressType)
            return;
        AddressType=newAddressType;
    }

    public static Address Empty => new("", _geometryFactory.CreatePoint(new Coordinate(0, 0)), AddressType.MainLocation);

    public static Result<Address> Create(string street, string latitude, string longitude, AddressType addressType, string? comment = null)
    {
        if(string.IsNullOrWhiteSpace(street))
            return new Error("Street cannot be null or empty", nameof(street));

        if(!double.TryParse(latitude, out var lat))
            return new Error("Invalid latitude format", nameof(latitude));
        if(!double.TryParse(longitude, out var lon))
            return new Error("Invalid longitude format", nameof(longitude));

        if(lat<MIN_LATITUDE||lat>MAX_LATITUDE)
            return new Error(nameof(latitude),
                $"Latitude must be between {MIN_LATITUDE} and {MAX_LATITUDE} for Romania");

        if(lon<MIN_LONGITUDE||lon>MAX_LONGITUDE)
            return new Error(nameof(longitude),
                $"Longitude must be between {MIN_LONGITUDE} and {MAX_LONGITUDE} for Romania");

        var point = _geometryFactory.CreatePoint(new Coordinate(lon, lat));

        return new Address(street, point, addressType, comment);
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

    public Result<AddressDto> ToDto(AddressType addressType)
    {
        return AddressDto.Create(Street, Coordinate?.X.ToString()??"", Coordinate?.Y.ToString()??"", addressType, Comment);
    }

    public Result<Address> Update(
        string? street = null,
        string? latitude = null,
        string? longitude = null,
        AddressType? addressType = null,
        string? comment = null)
    {
        if(street is not null)
        {
            if(string.IsNullOrWhiteSpace(street))
                return new Error("Street cannot be null or empty", nameof(street));
            Street=street;
        }
        if(latitude is not null&&longitude is not null)
        {
            if(!double.TryParse(latitude, out var lat))
                return new Error("Invalid latitude format", nameof(latitude));
            if(!double.TryParse(longitude, out var lon))
                return new Error("Invalid longitude format", nameof(longitude));
            if(lat<MIN_LATITUDE||lat>MAX_LATITUDE)
                return new Error(nameof(latitude),
                    $"Latitude must be between {MIN_LATITUDE} and {MAX_LATITUDE} for Romania");
            if(lon<MIN_LONGITUDE||lon>MAX_LONGITUDE)
                return new Error(nameof(longitude),
                    $"Longitude must be between {MIN_LONGITUDE} and {MAX_LONGITUDE} for Romania");
            Coordinate=_geometryFactory.CreatePoint(new Coordinate(lon, lat));
        }
        if(addressType.HasValue)
        {
            AddressType=addressType.Value;
        }
        if(comment is not null)
        {
            Comment=comment;
        }
        return this;
    }

    internal void SetSchool(School school)
    {
        School=school;
    }
}