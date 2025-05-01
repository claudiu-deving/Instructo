using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Shared;

namespace Application.Roles.Queries.GetRoles;

public class GetRolesQuery: ICommand<Result<ApplicationRole>>;

public class GetRolesQueryHandler : ICommandHandler<GetRolesQuery, Result<ApplicationRole>>
{
    public Task<Result<ApplicationRole>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}