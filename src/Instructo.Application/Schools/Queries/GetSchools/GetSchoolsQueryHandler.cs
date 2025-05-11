using Application.Abstractions.Messaging;
using Domain.Dtos.Image;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler(IQueryRepository<School, SchoolId> repository)
    : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<SchoolReadDto>>>
{
    public async Task<Result<IEnumerable<SchoolReadDto>>> Handle(GetSchoolsQuery request,
        CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetAllAsync();
        if (repositoryRequest.IsError)
            return Result<IEnumerable<SchoolReadDto>>.Failure(repositoryRequest.Errors);
        return repositoryRequest.Map(x => Map(x));
    }

    private IEnumerable<SchoolReadDto> Map(IEnumerable<School> schools)
    {
        return schools.Select(s => new SchoolReadDto(
            s.Id.Id,
            s.Name,
            s.CompanyName,
            s.Email,
            s.PhoneNumber,
            s.PhoneNumbersGroups?.Select(x => x.ToDto()),
            s.Icon?.ToDto() ?? new ImageReadDto(),
            [.. s.WebsiteLinks.Select(x => x.ToDto())],
            VehicleCategories: s.VehicleCategories.Select(x => x.ToDto()).ToList(),
            BussinessHours: s.BussinessHours?.BussinessHoursEntries ?? [],
            Certificates: s.Certificates.Select(x => x.ToDto()).ToList()));
    }
}