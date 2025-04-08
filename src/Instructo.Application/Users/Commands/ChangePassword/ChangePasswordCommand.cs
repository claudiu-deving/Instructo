using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(string Email, string CurrentPassword, string NewPassword) : ICommand<Result<string>>;
