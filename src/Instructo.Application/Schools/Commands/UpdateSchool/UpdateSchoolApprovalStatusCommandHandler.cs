using Application.Abstractions.Messaging;

using Domain.Dtos.School;
using Domain.Interfaces;
using Domain.Mappers;
using Domain.Shared;

namespace Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolApprovalStatusCommandHandler(
    ISchoolQueriesRepository repository,
    ISchoolCommandRepository schoolCommandRepository
)
    : ICommandHandler<UpdateSchoolApprovalStatusCommand, Result<SchoolDetailReadDto?>>
{
    public async Task<Result<SchoolDetailReadDto?>> Handle(
        UpdateSchoolApprovalStatusCommand request,
        CancellationToken cancellationToken)
    {
        var schoolRetrievalRequest = await repository.GetByIdAsync(request.School);

        if(schoolRetrievalRequest.IsError)
            return schoolRetrievalRequest.Errors;
        if(schoolRetrievalRequest.Value is not { } school)
        {
            return Result<SchoolDetailReadDto?>.Success(null);
        }

        var approvalRequest = await schoolCommandRepository.SetApprovalState(request.School, request.IsApproved);
        return approvalRequest.Match(
            school => school.ToDetailedReadDto(),
            Result<SchoolDetailReadDto?>.Failure
        );
    }
}