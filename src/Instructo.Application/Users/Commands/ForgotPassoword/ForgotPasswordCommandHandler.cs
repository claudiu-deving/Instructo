using Application.Abstractions.Messaging;

using Domain.Shared;

namespace Application.Users.Commands.ForgotPassoword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("The forgot password is not yet implemented");
    }
}
