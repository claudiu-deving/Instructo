using MediatR;

namespace Instructo.Domain.Shared;

public sealed class ValidationResult :
    Result<string>, IValidationResult
{
    public Error[] Errors { get; }

    private ValidationResult(Error[] errors) : base(errors)
    {
        Errors=errors;
    }

    public static ValidationResult WithErrors(Error[] errors) => new(errors);

}

public sealed class ValidationResult<TValue>
    : Result<TValue>, IValidationResult
{
    public Error[] Errors { get; }
    private ValidationResult(Error[] errors) : base(errors)
    {
        Errors=errors;
    }

    public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
}