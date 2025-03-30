using Instructo.Application.Users.CreateUser;
using Instructo.Application.Users.Delete;
using Instructo.Application.Users.Query;
using Instructo.Application.Users.UpdateUser;
using Instructo.Domain.Dtos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Instructo.Api.Endpoints;

public static class UserEndpoint
{

    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/users").WithTags("Users");


        group.MapGet("/", GetAllUsers).WithName("Get all users");
        group.MapGet("/{id}", GetUserById).WithName("Get a User By Id");
        group.MapPost("/", CreateUser).WithName("Create User");
        group.MapPatch("/{id}", UpdateUser).WithName("Update User");
        group.MapDelete("/by-email/{email}", DeleteUserByEmail);
        group.MapDelete("/{id}", DeleteUserById);
        group.RequireAuthorization("IronMan").WithOpenApi();
        return app;
    }

    private static async Task<IResult> GetAllUsers([FromServices] ISender sender, [AsParameters] GetUsersQueryParameters parameters)
    {
        var query = new GetUsersBySuperQuery();
        var userRequest = await sender.Send(query);
        parameters=parameters with { PageNumber=parameters.PageNumber==0 ? 1 : parameters.PageNumber };
        parameters=parameters with { PageSize=parameters.PageSize>50 ? 50 : parameters.PageSize };
        return userRequest.Match<IResult>(
                   ok =>
                   {
                       if(!string.IsNullOrEmpty(parameters.SearchTerm))
                       {
                           ok= [.. ok.Where(x => x.Email.Contains(parameters.SearchTerm)||x.FirstName.Contains(parameters.SearchTerm)||x.LastName.Contains(parameters.SearchTerm))];
                       }
                       if(!string.IsNullOrEmpty(parameters.Role))
                       {
                           ok= [.. ok.Where(x => x.Role==parameters.Role)];
                       }
                       if(parameters.IsActive.HasValue)
                       {
                           ok= [.. ok.Where(x => x.IsActive==parameters.IsActive.Value)];
                       }
                       ok= [.. ok.Skip((parameters.PageNumber-1)*parameters.PageSize).Take(parameters.PageSize)];

                       return TypedResults.Ok(ok);
                   },
                   nok =>
                   {
                       return TypedResults.BadRequest(new { errors = nok.ToList() });
                   });

    }

    private static async Task<IResult> GetUserById([FromServices] ISender sender, string id)
    {
        var query = new GetUserByIdBySuperQuery(id);
        var userRequest = await sender.Send(query);
        return userRequest.Match<IResult>(
                   ok =>
                   {
                       return TypedResults.Ok(ok);
                   },
                   nok =>
                   {
                       return TypedResults.BadRequest(new { errors = nok.ToList() });
                   });
    }

    private static async Task<IResult> CreateUser([FromServices] ISender sender, ILogger<Program> logger, RegisterUserCommand createUserCommand)
    {
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

    private static async Task<IResult> UpdateUser([FromServices] ISender sender, ILogger<Program> logger, string id, [FromBody] UserUpdateDto userUpdateDto)
    {
        logger.LogInformation("Registering by admin for user: {user}", userUpdateDto);
        var userRetrievalRequest = await sender.Send(new GetUserByIdBySuperQuery(id));
        return await userRetrievalRequest.Match<Task<IResult>>(async ok =>
            {
                var userDto = await sender.Send(new UpdateUserCommand(id, userUpdateDto));
                return userDto.Match<IResult>(ok =>
                  {
                      return TypedResults.Ok($"/api/users/{ok}");
                  },
                  nok =>
                  {
                      return TypedResults.BadRequest(nok);
                  });
            },
            async nok =>
            {
                return await Task.Run(() =>
                {
                    return TypedResults.NotFound(new
                    {
                        errors = nok.ToList()
                    });
                });
            });
    }

    private static async Task<IResult> DeleteUserById([FromServices] ISender sender, string id)
    {
        var userRetrievalRequest = await sender.Send(new GetUserByIdBySuperQuery(id));
        return await userRetrievalRequest.Match<Task<IResult>>(async ok =>
        {
            var userDto = await sender.Send(new DeleteUserByIdCommand(id));
            return userDto.Match<IResult>(ok =>
              {
                  return TypedResults.Ok(userDto);
              },
              nok =>
              {
                  return TypedResults.BadRequest(nok);
              });
        },
        async nok =>
        {
            return await Task.Run(() =>
            {
                return TypedResults.NotFound(new
                {
                    errors = nok.ToList()
                });
            });
        });
    }
    private static async Task<IResult> DeleteUserByEmail([FromServices] ISender sender, string email)
    {
        var userRetrievalRequest = await sender.Send(new GetUserByEmailCommand(email));
        return await userRetrievalRequest.Match<Task<IResult>>(async ok =>
        {
            var userDto = await sender.Send(new DeleteUserByIdCommand(ok.Id));
            return userDto.Match<IResult>(ok =>
            {
                return TypedResults.Ok($"{userDto}");
            },
              nok =>
              {
                  return TypedResults.BadRequest(nok);
              });
        },
        async nok =>
        {
            return await Task.Run(() =>
            {
                return TypedResults.NotFound(new
                {
                    errors = nok.ToList()
                });
            });
        });
    }
}
