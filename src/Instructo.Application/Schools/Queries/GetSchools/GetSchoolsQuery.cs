using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Shared;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQuery(bool IsAdmin, bool RequestWithDetails, GetSchoolsQueryParameters Parameters) : ICommand<Result<IEnumerable<ISchoolReadDto>>>
{
    public bool IsAdmin { get; set; } = IsAdmin;
    public bool RequestWithDetails { get; } = RequestWithDetails;
    public GetSchoolsQueryParameters Parameters { get; } = Parameters;
}
