using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Role) : ICommand<Result<string>>;
