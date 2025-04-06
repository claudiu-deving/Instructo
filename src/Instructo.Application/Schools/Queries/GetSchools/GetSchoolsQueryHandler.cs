using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Dtos.Image;
using Instructo.Domain.Dtos.School;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;
using Instructo.Domain.ValueObjects;

namespace Instructo.Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler(IQueryRepository<School, SchoolId> repository) : ICommandHandler<GetSchools.GetSchoolsQuery, Result<IEnumerable<SchoolReadDto>>>
{
    public async Task<Result<IEnumerable<SchoolReadDto>>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetAllAsync();
        if(repositoryRequest.IsError)
        {
            return Result<IEnumerable<SchoolReadDto>>.Failure(repositoryRequest.Errors);
        }
        return repositoryRequest.Map(x => x?.Select(s => new SchoolReadDto
        {
            Id=s.Id.Id,
            Name=s.Name,
            CompanyName=s.CompanyName,
            Email=s.Email,
            PhoneNumber=s.PhoneNumber,
            Links= [.. s.WebsiteLinks.Select(x => x.ToDto())],
            IconData=s.Icon?.ToDto()??new ImageReadDto(),
        })?? []);
    }
}
