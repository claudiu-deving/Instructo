namespace Domain.Shared;



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
    public Error[] Errors => IsError ? _errors : throw new InvalidOperationException("You cannot retrieve the Errors of a Successful result");

    private readonly TValue? _value;
    public TValue? Value => IsError
        ? throw new InvalidOperationException("You cannot retrieve the value of an Failure result")
        : _value;

    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);
    public static implicit operator Result<TValue>(Error[] errors) => new Result<TValue>(errors);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Error[], TResult> failure) =>
        !IsError ? success(Value!) : failure(Errors!);

    public override string ToString() => Match(
            success: value => value?.ToString()??string.Empty,
            failure: error => string.Join(Environment.NewLine, error.ToList())??string.Empty);

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(params Error[] errors) => new(errors);
    public static Result<TValue> WithErrors(Error[] errors) => new(errors);

    public Result<TOut> Map<TOut>(Func<TValue, TOut> mapper)
    {
        if(IsError)
            return Result<TOut>.Failure(Errors);

        return Result<TOut>.Success(mapper(Value!));
    }

    public TValue ValueOrDefault(TValue defaultValue) =>
    IsError ? defaultValue : Value!;

    public TValue ValueOrThrow() =>
        IsError ? throw new InvalidOperationException($"Cannot access value of error result: {ToString()}") : Value!;

    public bool TryGetValue(out TValue? value)
    {
        value=Value;
        return !IsError;
    }
}
public static class ResultExtensions
{
    public static Result<TOut> Then<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> next) =>
        result.IsError ? Result<TOut>.Failure(result.Errors) : next(result.Value!);


    public static Result<TValue> OnSuccess<TValue>(
        this Result<TValue> result,
        Action<TValue> action)
    {
        if(!result.IsError)
            action(result.Value!);

        return result;
    }

    public static Result<TValue> OnError<TValue>(
        this Result<TValue> result,
        Action<Error[]> action)
    {
        if(result.IsError)
            action(result.Errors);

        return result;
    }
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
    this Task<Result<TIn>> resultTask,
    Func<TIn, TOut> mapper)
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

}