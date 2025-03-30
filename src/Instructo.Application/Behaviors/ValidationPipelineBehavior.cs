
using FluentValidation;

using Instructo.Domain.Shared;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Instructo.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators=validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(!_validators.Any())
        {
            return await next();
        }
        var requestName = typeof(TRequest).Name;

        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(request, cancellationToken)));
        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error!=null)
            .Select(error => new Error(error.PropertyName, error.ErrorMessage))
            .Distinct()
            .ToArray();

        if(errors.Length!=0)
        {
            return CreateValidationResult<TResponse>(errors);
        }
        else
        {
            return await next();
        }
    }

    public static TResult CreateValidationResult<TResult>(Error[] errors) where TResult : class
    {
        return (ValidationResult.WithErrors(errors) as TResult)!;
    }
}
