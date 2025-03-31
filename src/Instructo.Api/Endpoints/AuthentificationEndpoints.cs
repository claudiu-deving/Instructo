using Instructo.Application.Users.Commands.ChangePassword;
using Instructo.Application.Users.Commands.ForgotPassoword;
using Instructo.Application.Users.Commands.LoginUser;
using Instructo.Application.Users.Commands.RegisterUser;
using Instructo.Domain.Dtos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Instructo.Api.Endpoints;

public static class AuthentificationEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth").WithTags("Authentification");
        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/change-password", ChangePassword);
        return app;
    }

    private static async Task<IResult> ChangePassword([FromServices] ISender sender, [FromServices] ILogger logger, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var changePasswordCommand = new ChangePasswordCommand(changePasswordDto.Email, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            var changePasswordRequest = await sender.Send(changePasswordCommand);
            return changePasswordRequest.Match<IResult>(ok =>
            {
                return TypedResults.Ok();
            }, nok =>
            {
                return TypedResults.BadRequest(new { errors = nok.ToList() });
            });
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error changing password: {Email}", changePasswordDto.Email);
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> Login([FromServices] ISender sender, [FromServices] ILogger<Program> logger, LoginDto loginDto)
    {
        try
        {
            var request = new LoginUserCommand(loginDto.Email, loginDto.Password);
            var loginRequest = await sender.Send(request);
            return loginRequest.Match<IResult>(returnedToken =>
             {
                 return TypedResults.Ok(returnedToken);
             }, errors =>
             {
                 return TypedResults.Unauthorized();
             });

        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error logging in for user: {Email}", loginDto.Email);
            return TypedResults.InternalServerError();
        }
    }
    private static async Task<IResult> Register([FromServices] ISender sender, [FromServices] ILogger<Program> logger, RegisterUserDto registerDto)
    {
        try
        {
            var createUserCommand = new RegisterUserCommand(
                registerDto.Email,
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.PhoneNumber,
                registerDto.Password,
                registerDto.Role);

            logger.LogInformation("Registering by admin for user: {Username}", createUserCommand.Email);
            var response = await sender.Send(createUserCommand);

            var result = response.Match<IResult>(
                     ok =>
                     {
                         return TypedResults.Created($"/api/users/{ok}");
                     },
                     nok =>
                     {
                         return TypedResults.BadRequest(new { errors = nok.ToList() });
                     });

            return result;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error registering user");
            return TypedResults.InternalServerError();
        }
    }

    private static async Task<IResult> ForgotPassword([FromServices] ISender sender, [FromServices] ILogger logger, ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var forgotPasswordCommand = new ForgotPasswordCommand(forgotPasswordDto.Email);
            var result = await sender.Send(forgotPasswordCommand);
            return result.Match<IResult>(ok =>
            {
                return TypedResults.Ok();
            }, nok =>
            {
                return TypedResults.BadRequest(new { errors = nok.ToList() });
            });
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error forgot password for user: {Email}", forgotPasswordDto.Email);
            return TypedResults.InternalServerError();
        }
    }
}
