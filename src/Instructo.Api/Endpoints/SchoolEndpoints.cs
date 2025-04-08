using Application.Schools.Commands.DeleteSchool;
using Application.Schools.Queries.GetSchoolById;
using Application.Schools.Queries.GetSchools;

using Domain.Dtos.School;
using Domain.Shared;
using Domain.ValueObjects;

using Application.Schools.Commands.CreateSchool;

using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class SchoolEndpoints
{
    public static WebApplication MapSchoolEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/schools").WithTags("Schools");
        group.MapGet("/", GetAllSchools).WithName("Get all schools").AllowAnonymous();
        group.MapGet("/{id}", GetSchoolById).WithName("Get a School By Id").AllowAnonymous();
        group.MapPost("/", CreateSchool).WithName("Create School");
        //group.MapPatch("/{id}", UpdateSchool).WithName("Update SchoolEntities");
        group.MapDelete("/{id}", DeleteSchool);
        group.WithOpenApi();
        return app;
    }
    private static async Task<IResult> DeleteSchool(
        HttpContext context,
        [FromServices] ISender sender,
        Guid id)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null)
            return TypedResults.Unauthorized();
        var deletionRequest = await sender.Send(new DeleteSchoolCommand(SchoolId.CreateNew(id), userId));
        if(deletionRequest.IsError)
            return TypedResults.BadRequest(new { errors = deletionRequest.Errors.ToList() });
        else
        {
            return TypedResults.Ok();
        }
    }

    private static async Task<IResult> CreateSchool([FromServices] ISender sender, [FromBody] CreateSchoolCommandDto createSchoolCommand)
    {
        var errors = new List<Error>();
        CreateSchoolCommand? command = null;
        CreateSchoolCommand.Create(createSchoolCommand)
           .OnSuccess(value => command=value)
           .OnError(errors.AddRange);

        if(errors.Count>0||command is null)
            return TypedResults.BadRequest(new { errors = errors.ToList() });

        var userRequest = await sender.Send(command);
        return userRequest.Match<IResult>(
            ok => TypedResults.Created($"/api/schools/{ok.Id}", ok),
            nok => TypedResults.BadRequest(new { errors = nok.ToList() }));
    }

    private static async Task<IResult> GetSchoolById([FromServices] ISender sender, Guid id)
    {
        var query = new GetSchoolByIdQuery(id);
        var userRequest = await sender.Send(query);
        return userRequest.Match<IResult>(
            ok =>
            {
                if(ok==new SchoolReadDto())
                    return TypedResults.NotFound();
                else
                {
                    return TypedResults.Ok(ok);
                }
            },
            nok => TypedResults.BadRequest(new { errors = nok.ToList() }));
    }

    private static async Task<IResult> GetAllSchools([FromServices] ISender sender, [AsParameters] GetSchoolsQueryParameters parameters)
    {
        var query = new GetSchoolsQuery();
        var userRequest = await sender.Send(query);
        parameters=parameters with { PageNumber=parameters.PageNumber==0 ? 1 : parameters.PageNumber };
        parameters=parameters with { PageSize=parameters.PageSize>50 ? 50 : parameters.PageSize };
        return userRequest.Match<IResult>(
            ok =>
            {
                if(!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    ok= [.. ok.Where(x =>
                    x.CompanyName.Contains(parameters.SearchTerm)||
                    x.Name.Contains(parameters.SearchTerm)||
                    x.Email.Contains(parameters.SearchTerm)||
                    x.PhoneNumber.Contains(parameters.SearchTerm))];
                }
                ok= [.. ok.Skip((parameters.PageNumber-1)*parameters.PageSize).Take(parameters.PageSize)];

                return TypedResults.Ok(ok);
            },
            nok =>
            {
                return TypedResults.BadRequest(new { errors = nok.ToList() });
            });
    }

}
