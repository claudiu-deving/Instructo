using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Shared;

namespace Application.Users.Queries.GetUsers;

public record GetUsersQuery : ICommand<Result<IEnumerable<ApplicationUser>>>;