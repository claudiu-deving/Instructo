using Domain.Shared;
namespace Domain.ValueObjects;

//AddressDto
public readonly record struct AddressDto
{
    public string Longitude { get; }
    public string Latitude { get; }
    public string Street { get; }
    public string? Comment { get; }
    private AddressDto(string value, string x, string y, string? comment = null)
    {
        Street=value;
        Longitude=x;
        Latitude=y;
        Comment=comment;
    }
    public static AddressDto Empty => new(string.Empty, string.Empty, string.Empty);
    public static Result<AddressDto> Create(string value, string longitude, string latitude, string? comment = null)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<AddressDto>.Failure([new Error("Address cannot be empty", value)]);

        return new AddressDto(value, longitude, latitude, comment);
    }
    public static implicit operator string(AddressDto value) => value.Street;
    public override string ToString()
    {
        return Street;
    }
}
