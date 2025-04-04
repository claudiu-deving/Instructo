using FluentValidation;

using Instructo.Domain.Interfaces;

namespace Instructo.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IUserQueries userRepository)
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MustAsync(async (email, _) =>
        {
            return await userRepository.IsEmailUnique(email);
        }).WithMessage("Email is already registered");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}
