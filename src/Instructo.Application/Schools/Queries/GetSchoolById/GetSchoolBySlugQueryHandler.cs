﻿using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Queries.GetSchoolById;

public class GetSchoolBySlugQueryHandler(ISchoolQueriesRepository repository) : ICommandHandler<GetSchoolBySlugQuery, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(GetSchoolBySlugQuery request, CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetBySlugAsync(request.Slug);
        if(repositoryRequest.IsError)
            return Result<SchoolReadDto>.Failure(repositoryRequest.Errors);
        return repositoryRequest.Map(x => x is null ? new SchoolReadDto() : x.ToReadDto());
    }
}
