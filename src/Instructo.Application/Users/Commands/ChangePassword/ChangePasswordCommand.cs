using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(string Email, string CurrentPassword, string NewPassword) : ICommand<Result<string>>;
