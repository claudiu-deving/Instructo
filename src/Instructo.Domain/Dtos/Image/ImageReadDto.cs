namespace Domain.Dtos.Image;

public readonly record struct ImageReadDto(string FileName, string Url, string ContentType, string Description)
{
    public static ImageReadDto Empty => new(string.Empty, string.Empty, string.Empty, string.Empty);
}