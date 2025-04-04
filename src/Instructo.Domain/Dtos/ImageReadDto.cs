namespace Instructo.Domain.Dtos;

public readonly record struct ImageReadDto(string FileName, string Url, string ContentType, string Description);