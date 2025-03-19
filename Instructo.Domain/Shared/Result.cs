namespace Instructo.Domain.Shared;

public readonly record struct Result<TValue>
{
    private Result(TValue value)
    {
        IsError=false;
        _value=value;
        _error=Error.None;
    }
    private Result(Error error)
    {
        IsError=true;
        _error=error;
        _value=default;
    }

    public bool IsError { get; }

    private readonly TValue? _value;
    private readonly Error _error;

    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);
    public static implicit operator Result<TValue>(Error error) => new Result<TValue>(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Error, TResult> failure) =>
        !IsError ? success(_value!) : failure(_error!);
}
