using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Dtos;
using Instructo.Domain.Interfaces;
using Instructo.Infrastructure.Migrations;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Instructo.Api.Endpoints;

public static class AuthentificationEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth").WithTags("Authentification");
        group.MapPost("/login", Login).RequireRateLimiting("Auth-RateLimiting");
        group.MapPost("/register", Register);
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/change-password", ChangePassword);
        return app;
    }

    private static async Task<IResult> ChangePassword([FromServices] IIdentityService identityService, ChangePasswordDto changePasswordDto)
    {
        var result = await identityService.ChangePassword(changePasswordDto);
        return !result.IsError ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    private static async Task<IResult> Login([FromServices] IIdentityService identityService, LoginDto loginDto)
    {
        var result = await identityService.LoginAsync(loginDto.Email, loginDto.Password);
        return result.Match<IResult>(ok =>
        {
            return TypedResults.Ok(ok);
        },
        nok =>
        {
            return TypedResults.Unauthorized();
        });
    }
    private static async Task<IResult> Register([FromServices] ISender sender, [FromServices] ILogger<Program> logger, RegisterUserDto registerDto)
    {
        logger.LogInformation("Registering for user: {Username}", registerDto.Email);
        RegisterUserCommand registerUserCommand = new RegisterUserCommand(
            registerDto.FirstName,
            registerDto.LastName,
            registerDto.Email,
            registerDto.Password,
            registerDto.PhoneNumber);
        var result = await sender.Send(registerUserCommand);
        if(result.IsError)
        {
            logger.LogInformation("Validation failed for user: {Username},with errors: {Errors}", registerDto.Email, result);
        }
        return !result.IsError ? TypedResults.Created("api/auth/register", result) : TypedResults.BadRequest(result);
    }

    private static async Task<IResult> ForgotPassword([FromServices] IIdentityService identityService, ForgotPasswordDto forgotPasswordDto)
    {
        var result = await identityService.ForgotPasswordAsync(forgotPasswordDto);
        return !result.IsError ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
