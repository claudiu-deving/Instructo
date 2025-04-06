using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.DeleteUser;

public record DeleteUserByIdCommand(Guid Id) : ICommand<Result<string>>;
