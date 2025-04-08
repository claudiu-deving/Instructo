using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserUpdate.FirstName).Must(NotBeEmptyIfNotNull()).WithMessage("First Name cannot be empty");
        RuleFor(x => x.UserUpdate.LastName).Must(NotBeEmptyIfNotNull()).WithMessage("Last Name cannot be empty");
        RuleFor(x => x.UserUpdate.PhoneNumber).Must(NotBeEmptyIfNotNull()).WithMessage("Phone Number cannot be empty");
    }

    private static Func<string?, bool> NotBeEmptyIfNotNull()
    {
        return x =>
        {
            if(x is null)
                return true;
            else
            {
                return x!=""&&x!=" ";
            }
        };
    }
}