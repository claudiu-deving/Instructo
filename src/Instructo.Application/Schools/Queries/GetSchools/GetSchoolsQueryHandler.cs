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
    : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<SchoolDetailReadDto>>>
{
    public async Task<Result<IEnumerable<SchoolDetailReadDto>>> Handle(GetSchoolsQuery request,
        CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetAllDetailedAsync();
        if(repositoryRequest.IsError)
            return Result<IEnumerable<SchoolDetailReadDto>>.Failure(repositoryRequest.Errors);
        return repositoryRequest.Map(x => x);
    }

}