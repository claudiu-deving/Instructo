using MediatR;

namespace Instructo.Domain.Shared;

public class Result<TValue> : IResult<TValue>
{
    protected Result(TValue value)
    {
        IsError=false;
        _value=value;
        _errors= [Error.None];
    }
    protected Result(Error[] errors)
    {
        IsError=true;
        _errors=errors;
        _value=default;
    }

    public bool IsError { get; protected set; }
    private readonly Error[] _errors;
    private readonly TValue? _value;

    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);
    public static implicit operator Result<TValue>(Error[] errors) => new Result<TValue>(errors);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Error[], TResult> failure) =>
        !IsError ? success(_value!) : failure(_errors!);

    public override string ToString() => Match(
            success: value => value?.ToString()??string.Empty,
            failure: error => string.Join(Environment.NewLine, error.ToList())??string.Empty);

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(Error[] errors) => new(errors);
    public static Result<TValue> WithErrors(Error[] errors) => new(errors);


}
