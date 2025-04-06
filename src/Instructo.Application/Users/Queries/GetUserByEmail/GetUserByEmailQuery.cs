using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos.User;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Queries.GetUserByEmail;
public record GetUserByEmailQuery(string Email) : ICommand<Result<UserReadDto>>;
