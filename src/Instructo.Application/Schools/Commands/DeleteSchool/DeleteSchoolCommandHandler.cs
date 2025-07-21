using Application.Abstractions.Messaging;

using Domain.Entities.SchoolEntities;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Messager;

using Microsoft.Extensions.Logging;

namespace Application.Schools.Commands.DeleteSchool;

public class DeleteSchoolCommandHandler(
    ISchoolCommandRepository commandRepository,
   ISchoolQueriesRepository schoolQueryRepository,
    IUserQueriesRepository userQueryRepository,
    ILogger<DeleteSchoolCommandHandler> logger) : ICommandHandler<DeleteSchoolCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(request.UserId, out var userId))
        {
            logger.LogError("Unauthentificated user should not reach this point, missing Authorization policies?");
            return Result<Unit>.WithErrors([
                new Error("InvalidInput", $"The input guid {request.UserId} of user is not valid")
            ]);
        }


        var user = await userQueryRepository.GetUserByIdAsync(userId);
        if(user is null)
        {
            logger.LogError("User not found in the database but passed the Authorization policy!!?");
            return Result<Unit>.WithErrors([new Error("NotFound", $"The user {userId} not found")]);
        }

        School? existingSchool = null;
        List<Error> errors = [];
        var existingSchoolRequest = await schoolQueryRepository.GetByIdAsync(request.Id);
        existingSchoolRequest.OnSuccess(school => existingSchool=school)
            .OnError(errors.AddRange);
        if(errors.Count!=0||existingSchool is null)
        {
            logger.LogError("The school {school} doesn't exist", new { school = existingSchool?.Id });
            return Result<Unit>.WithErrors([.. errors]);
        }

        if(existingSchool.OwnerId!=user.Id)
        {
            logger.LogError("Non authorized user {User} attempted to delete a {School}", new { User = user.Id },
                new { School = existingSchool.Id });
            return Result<Unit>.WithErrors([
                new Error("Unauthorized", $"The user {userId} is not the owner of the school {existingSchool.Id}")
            ]);
        }


        var deletionRequest = await commandRepository.DeleteAsync(existingSchool);
        deletionRequest.OnError(errors.AddRange);
        if(errors.Count!=0)
            return Result<Unit>.WithErrors([.. errors]);
        return Result<Unit>.Success(Unit.Value);
    }
}