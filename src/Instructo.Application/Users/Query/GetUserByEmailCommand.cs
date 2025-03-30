using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Query;
public record GetUserByEmailCommand(string Email) : ICommand<Result<UserReadDto>>;
