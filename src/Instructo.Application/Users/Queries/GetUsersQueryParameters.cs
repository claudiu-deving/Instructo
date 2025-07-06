namespace Application.Users.Queries;

public readonly record struct GetUsersQueryParameters(string? SearchTerm, string? Role, bool? IsActive, int PageNumber = 1, int PageSize = 50);
