namespace Application.Schools.Queries.GetSchools;

public readonly record struct GetSchoolsQueryParameters(string? SearchTerm, int PageNumber = 1, int PageSize = 50);
