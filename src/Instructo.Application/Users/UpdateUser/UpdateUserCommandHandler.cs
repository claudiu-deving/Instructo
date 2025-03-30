

using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;

namespace Instructo.Application.Users.UpdateUser;

public class UpdateUserCommandHandler(IIdentityService identityService) : ICommandHandler<UpdateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.UpdateUserAsync(request.Id,request.UserUpdate);
    }
}