using Application.Schools.Commands.CreateSchool;
using Application.Schools.Commands.DeleteSchool;
using Application.Schools.Commands.UpdateSchool;
using Application.Schools.Queries.GetSchoolById;
using Application.Schools.Queries.GetSchools;

using Domain.Dtos.School;
using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class SchoolEndpoints
{
    public static WebApplication MapSchoolEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/schools").WithTags("Schools");
        group.MapGet("/", GetAllSchools).WithName("Get all schools").AllowAnonymous();
        group.MapGet("/{slug}", GetSchoolById).WithName("Get a School By Slug").AllowAnonymous();
        group.MapPost("/", CreateSchool).WithName("Create School");
        group.MapPatch("/{id:guid}", UpdateSchool).WithName("Update SchoolEntities")
            .RequireAuthorization(ApplicationRole.IronMan.ToString());
        group.MapDelete("/{id:guid}", DeleteSchool);
        group.MapPatch("/approval/{id:guid}", MapUpdateStatus).RequireAuthorization(ApplicationRole.IronMan.ToString());
        group.WithOpenApi();
        return app;
    }

    private static async Task<IResult> MapUpdateStatus([FromServices] ISender sender, Guid id,
        [FromBody] UpdateApprovalStatusCommandDto createSchoolCommand)
    {
        var schoolId = SchoolId.CreateNew(id);
        var command = new UpdateSchoolApprovalStatusCommand(schoolId, createSchoolCommand.IsApproved);
        var updateRequest = await sender.Send(command);
        if(updateRequest.IsError)
            return TypedResults.BadRequest(new { errors = updateRequest.Errors.ToList() });
        return TypedResults.Ok();
    }


    private static async Task<IResult> UpdateSchool([FromServices] ISender sender, HttpContext context, Guid id,
        [FromBody] UpdateSchoolCommandDto createSchoolCommand)
    {
        var schoolId = SchoolId.CreateNew(id);
        if(context.User.FindFirstValue(ClaimTypes.NameIdentifier) is not { } guidValue||
            !Guid.TryParse(guidValue, out var guid))
            return TypedResults.Unauthorized();

        var updateRequest = await FlexContext.StartContextAsync()
            .Then(ctx => UpdateSchoolCommand.Create(createSchoolCommand, schoolId, guid))
            .Then(flexContext => sender.Send(flexContext.Get<UpdateSchoolCommand>()));
        if(!updateRequest.IsError)
            return Results.Ok(updateRequest.Value!.Get<SchoolReadDto>());
        return updateRequest.Errors.Any(x => x.Code=="NotFound") ? TypedResults.NotFound() : Results.Ok();
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
        return TypedResults.Ok();
    }

    private static async Task<IResult> CreateSchool([FromServices] ISender sender,
        [FromBody] CreateSchoolCommandDto createSchoolCommand)
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

    private static async Task<IResult> GetSchoolById([FromServices] ISender sender, string slug)
    {
        var query = new GetSchoolByIdQuery(slug);
        var userRequest = await sender.Send(query);
        return userRequest.Match<IResult>(
            ok =>
            {
                if(ok==new SchoolReadDto())
                    return TypedResults.NotFound();
                return TypedResults.Ok(ok);
            },
            nok => TypedResults.BadRequest(new { errors = nok.ToList() }));
    }

    private static async Task<IResult> GetAllSchools(HttpContext context, [FromServices] ISender sender,
        [AsParameters] GetSchoolsQueryParameters parameters)
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        var isAdmin = role is "IronMan";

        var query = new GetSchoolsQuery(isAdmin);
        var userRequest = await sender.Send(query);
        parameters=parameters with { PageNumber=parameters.PageNumber==0 ? 1 : parameters.PageNumber };
        parameters=parameters with { PageSize=parameters.PageSize>50 ? 50 : parameters.PageSize };
        return userRequest.Match<IResult>(
            ok =>
            {
                if(!string.IsNullOrEmpty(parameters.SearchTerm))
                    ok=
                    [
                        .. ok.Where(x =>
                            x.CompanyName.Contains(parameters.SearchTerm) ||
                            x.Name.Contains(parameters.SearchTerm) ||
                            x.Email.Contains(parameters.SearchTerm) ||
                            x.PhoneNumber.Contains(parameters.SearchTerm))
                    ];
                ok= [.. ok.Skip((parameters.PageNumber-1)*parameters.PageSize).Take(parameters.PageSize)];

                return TypedResults.Ok(ok);
            },
            nok => { return TypedResults.BadRequest(new { errors = nok.ToList() }); });
    }
}