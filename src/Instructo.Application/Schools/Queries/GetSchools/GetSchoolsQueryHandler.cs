using Application.Abstractions.Messaging;

using Domain.Dtos.Image;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler(IQueryRepository<School, SchoolId> repository) : ICommandHandler<GetSchoolsQuery, Result<IEnumerable<SchoolReadDto>>>
{
    public async Task<Result<IEnumerable<SchoolReadDto>>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var repositoryRequest = await repository.GetAllAsync();
        if(repositoryRequest.IsError)
            return Result<IEnumerable<SchoolReadDto>>.Failure(repositoryRequest.Errors);
        return repositoryRequest.Map(x => x?.Select(s => new SchoolReadDto(

           Id: s.Id.Id,
           Name: s.Name,
           CompanyName: s.CompanyName,
           Email: s.Email,
           PhoneNumber: s.PhoneNumber,
           PhoneNumberGroups: s.PhoneNumbersGroups.Select(x => x.ToDto()),
           IconData: s.Icon?.ToDto()??new ImageReadDto(),
            Links: [.. s.WebsiteLinks.Select(x => x.ToDto())],
            VehicleCategories: s.VehicleCategories.Select(x => x.ToDto()).ToList(),
            BussinessHours: s.BussinessHours?.BussinessHoursEntries?? [],
            Certificates: s.Certificates.Select(x => x.ToDto()).ToList()
        ))?? []);
    }
}
