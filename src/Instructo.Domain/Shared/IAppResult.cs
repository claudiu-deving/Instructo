namespace Domain.Shared;
public interface IAppResult
{
    bool IsError { get; }

}
public interface IResult<TValue> : IAppResult
{
    TResult Match<TResult>(Func<TValue, TResult> success, Func<Error[], TResult> failure);
}