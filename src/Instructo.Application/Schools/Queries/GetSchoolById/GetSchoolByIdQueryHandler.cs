using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Mappers;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Application.Schools.Queries.GetSchoolById;

public class GetSchoolByIdQueryHandler(IQueryRepository<School, SchoolId> repository) : ICommandHandler<GetSchoolByIdQuery, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetByIdAsync(SchoolId.CreateNew(request.Id));
        if(repositoryRequest.IsError)
        {
            return Result<SchoolReadDto>.Failure(repositoryRequest.Errors);
        }
        return repositoryRequest.Map(x => x is null ? new SchoolReadDto() : x.ToReadDto());
    }
}
