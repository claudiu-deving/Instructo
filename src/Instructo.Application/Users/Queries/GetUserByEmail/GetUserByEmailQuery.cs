using Application.Abstractions.Messaging;

using Domain.Dtos.User;
using Domain.Entities;
using Domain.Shared;

namespace Application.Users.Queries.GetUserByEmail;
public record GetUserByEmailQuery(string Email) : ICommand<Result<ApplicationUser>>;
