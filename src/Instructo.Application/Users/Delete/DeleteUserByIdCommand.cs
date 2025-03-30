using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Delete;

public record DeleteUserByIdCommand(string Id) : ICommand<Result<string>>;
