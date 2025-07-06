using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Shared;

namespace Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : ICommand<Result<ApplicationUser>>;