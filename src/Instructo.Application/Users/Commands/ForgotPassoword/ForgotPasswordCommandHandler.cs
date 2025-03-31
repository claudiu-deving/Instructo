using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.ForgotPassoword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("The forgot password is not yet implemented");
    }
}
