using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Dtos.User;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, UserUpdateDto UserUpdate) : ICommand<Result<string>>;
