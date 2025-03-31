using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Application.Abstractions.Messaging;
using Instructo.Domain.Shared;

namespace Instructo.Application.Users.Commands.ForgotPassoword;

public record ForgotPasswordCommand(string Email) : ICommand<Result<string>>;
