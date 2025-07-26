using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler(ISchoolQueriesRepository repository)
    : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<ISchoolReadDto>>>
{
    public Task<Result<IEnumerable<ISchoolReadDto>>> Handle(GetSchoolsQuery request,
        CancellationToken cancellationToken)
    {
        Func<School, bool>? filter = null;
        if(!string.IsNullOrEmpty(request.Parameters.SearchTerm))
        {
            filter=(x) =>
            {
                var result = x.CompanyName==request.Parameters.SearchTerm||
                               x.Name.Contains(request.Parameters.SearchTerm)||
                               x.Email.Contains(request.Parameters.SearchTerm)||
                               x.PhoneNumber.Value.Contains(request.Parameters.SearchTerm);
                return result;
            };
        }

        var repositoryRequest = repository.GetAll(
            filter,
            pageNumber: request.Parameters.PageNumber,
            pageSize: request.Parameters.PageSize);
        if(repositoryRequest.IsError)
            return Task.FromResult(Result<IEnumerable<ISchoolReadDto>>.Failure(repositoryRequest.Errors));
        return Task.FromResult(repositoryRequest);

    }
}