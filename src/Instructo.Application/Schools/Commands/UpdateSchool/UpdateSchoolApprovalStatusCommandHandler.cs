using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolApprovalStatusCommandHandler(
    ISchoolQueriesRepository repository,
    ISchoolCommandRepository schoolCommandRepository
)
    : ICommandHandler<UpdateSchoolApprovalStatusCommand, Result<SchoolReadDto>>
{
    public async Task<Result<SchoolReadDto>> Handle(UpdateSchoolApprovalStatusCommand request,
        CancellationToken cancellationToken)
    {
        var schoolRetrievalRequest = await repository.GetByIdAsync(request.School);

        if(schoolRetrievalRequest.IsError)
            return Result<SchoolReadDto>.Failure(schoolRetrievalRequest.Errors);

        var approvalRequest = await schoolCommandRepository.SetApprovalState(request.School, request.IsApproved);
        return approvalRequest.Match(
            school => school.ToReadDto(),
            Result<SchoolReadDto>.Failure
        );
    }
}