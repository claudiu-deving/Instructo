using Application.Abstractions.Messaging;

using Domain.Dtos.Image;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler(ISchoolQueriesRepository repository)
    : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<ISchoolReadDto>>>
{
    public async Task<Result<IEnumerable<ISchoolReadDto>>> Handle(GetSchoolsQuery request,
        CancellationToken cancellationToken)
    {
        Func<School, bool>? filter = null;
        if(!string.IsNullOrEmpty(request.Parameters.SearchTerm))
        {
            filter=(x) => x.CompanyName==request.Parameters.SearchTerm||
                           x.Name.Contains(request.Parameters.SearchTerm)||
                           x.Email.Contains(request.Parameters.SearchTerm)||
                           x.PhoneNumber.Value.Contains(request.Parameters.SearchTerm);
        }

        var repositoryRequest = repository.GetAll(
            filter,
            pageNumber: request.Parameters.PageNumber,
            pageSize: request.Parameters.PageSize,
            request.RequestWithDetails);
        if(repositoryRequest.IsError)
            return Result<IEnumerable<ISchoolReadDto>>.Failure(repositoryRequest.Errors);
        return repositoryRequest;

    }
}