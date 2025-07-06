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
    : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<SchoolReadDto>>>
{
    public async Task<Result<IEnumerable<SchoolReadDto>>> Handle(GetSchoolsQuery request,
        CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetAllAsync();
        if(repositoryRequest.IsError)
            return Result<IEnumerable<SchoolReadDto>>.Failure(repositoryRequest.Errors);
        return repositoryRequest.Map(x => Map(x, request.IsAdmin));
    }

    private static IEnumerable<SchoolReadDto> Map(IEnumerable<School> schools, bool isAdmin)
    {
        return schools.Where(s => s.IsApproved!=isAdmin).Select(s => new SchoolReadDto(
            s.Id.Id,
            s.Name,
            s.CompanyName,
            s.Email,
            s.PhoneNumber,
            s.Slug,
            s.PhoneNumbersGroups?.Select(x => x.ToDto())?? [],
            s.Icon?.ToDto()??new ImageReadDto(),
            [.. s.WebsiteLinks.Select(x => x.ToDto())],
            VehicleCategories: [.. s.VehicleCategories.Select(x => x.ToDto())],
            BussinessHours: s.BussinessHours?.BussinessHoursEntries?? [],
            Certificates: [.. s.Certificates.Select(x => x.ToDto())]));
    }
}