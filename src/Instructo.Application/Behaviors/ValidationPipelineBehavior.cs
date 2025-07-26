
using System.Reflection;

using Domain.Shared;

using FluentValidation;

using Messager;

namespace Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(!validators.Any())
            return await next(cancellationToken);
        var requestName = typeof(TRequest).Name;

        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(request, cancellationToken)));
        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error!=null)
            .Select(error => new Error(error.PropertyName, error.ErrorMessage))
            .Distinct()
            .ToArray();

        if(errors.Length!=0)
            return CreateValidationResult<TResponse>(errors);
        else
        {
            return await next(cancellationToken);
        }
    }

    public static TResult CreateValidationResult<TResult>(Error[] errors) where TResult : class
    {
        // Check if TResult is or derives from Result<T>
        if(typeof(IAppResult).IsAssignableFrom(typeof(TResult)))
        {
            // Create an instance of TResult using its constructor that takes errors
            // You would need to ensure TResult has such a constructor
            // Or use a factory method approach

            // For simplicity, assuming all derived classes implement a static WithErrors method:
            var method = typeof(TResult).GetMethod("WithErrors",
                BindingFlags.Public|BindingFlags.Static);

            if(method!=null)
                return (TResult)method.Invoke(null, [errors])!;
        }

        throw new InvalidOperationException($"Type {typeof(TResult).Name} does not support validation errors");
    }
}
