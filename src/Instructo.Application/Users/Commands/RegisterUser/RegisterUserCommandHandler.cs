using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Instructo.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(IIdentityService identityService, ILogger<RegisterUserCommandHandler> logger) : ICommandHandler<RegisterUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await identityService.GetUserByEmailAsync(request.Email);
        if(existingUser is not null)
        {
            logger.LogInformation("User registration failure due to previously existing user email for : {Email}", request.Email);
            return Result<string>.Failure([new Error("Register-User", "User already exists")]);
        }
        var registrationRequest = await identityService.CreateAsync(new ApplicationUser()
        {
            Email=request.Email,
            FirstName=request.FirstName,
            LastName=request.LastName,
            PhoneNumber=request.PhoneNumber
        }, request.Password);


        if(registrationRequest.IsError)
        {
            logger.LogError("Request for registration failed for: {Email}  with {errors}:\r\n", request.Email, registrationRequest.ToString());
            return Result<string>.Failure([new Error("Register-User", "User registration failed")]);
        }
        var registeredUser = await identityService.GetUserByEmailAsync(request.Email);
        if(registeredUser is null)
        {
            logger.LogError("User retrieval failed after registration");
            return Result<string>.Failure([new Error("Register-User", "User retrieval failed after registration")]);
        }
        var roleAssignmentRequest = await identityService.AddToRoleAsync(registeredUser, request.Role);
        if(roleAssignmentRequest.IsError)
        {
            logger.LogError("Role assignment failed after user registration succeded for: {Email} with {errors}:\r\n", request.Email, roleAssignmentRequest.ToString());
            return Result<string>.Failure([new Error("Register-User", "Role assignment failed after user registration succeded")]);
        }

        var token = await identityService.GenerateJwtTokenAsync(registeredUser);
        return Result<string>.Success(token);
    }
}
