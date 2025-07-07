using Domain.Shared;

namespace Domain.ValueObjects;

public readonly record struct CityDto
{
    public string Value { get; }
    private CityDto(string value) =>
        Value=value;
    public static CityDto Empty => new(string.Empty);
    public static Result<CityDto> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<CityDto>.Failure([new Error("City cannot be empty", value)]);
        if(value.Length>100)
            return Result<CityDto>.Failure([new Error("City cannot be longer than 100 characters", value)]);
        return new CityDto(value);
    }
    public static CityDto Wrap(string value) => new(value);
    public static implicit operator string(CityDto value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
