using Domain.Enums;
using Domain.Shared;
namespace Domain.ValueObjects;

//AddressDto
public readonly record struct AddressDto
{

    public string Longitude { get; init; } = string.Empty;
    public string Latitude { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string? Comment { get; init; }

    public AddressType AddressType { get; init; }

    public AddressDto() { }
    private AddressDto(string value, string x, string y, AddressType addressType, string? comment = null)
    {
        Street=value;
        Longitude=y;
        Latitude=x;
        Comment=comment;
        AddressType=addressType;
    }
    public static AddressDto Empty => new(string.Empty, string.Empty, string.Empty, AddressType.LessonStart);
    public static Result<AddressDto> Create(string value, string longitude, string latitude, AddressType addressType, string? comment = null)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<AddressDto>.Failure([new Error("Address cannot be empty", value)]);

        return new AddressDto(value, longitude, latitude, addressType, comment);
    }
    public static implicit operator string(AddressDto value) => value.Street;
    public override string ToString()
    {
        return Street;
    }
}
