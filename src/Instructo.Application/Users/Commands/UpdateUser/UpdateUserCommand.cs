using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(string Id, UserUpdateDto UserUpdate) : ICommand<Result<string>>;
