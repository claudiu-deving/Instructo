using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Application.Abstractions.Messaging;

public interface ICommand<T> : IRequest<T>
{
}

public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>
{
}

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}