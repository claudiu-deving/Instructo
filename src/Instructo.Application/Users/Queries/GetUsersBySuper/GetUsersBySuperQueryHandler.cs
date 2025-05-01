using Domain.Dtos.User;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUsersBySuper;

public class GetUsersBySuperQueryHandler : IRequestHandler<GetUsersBySuperQuery, Result<IEnumerable<ApplicationUser>>>
{
    private readonly IUserQueries _instructorQueries;

    public GetUsersBySuperQueryHandler(IUserQueries instructorQueries)
    {
        _instructorQueries=instructorQueries;
    }
    public async Task<Result<IEnumerable<ApplicationUser>>> Handle(GetUsersBySuperQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _instructorQueries.GetUsersBySuper();
            return Result<IEnumerable<ApplicationUser>>.Success(users);
        }
        catch(Exception ex)
        {
            return Result<IEnumerable<ApplicationUser>>.Failure([new Error("Error", ex.Message)]);
        }
    }
}