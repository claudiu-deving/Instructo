
namespace Instructo.Domain.Shared;
public interface IResult
{
    bool IsError { get; }
}
public interface IResult<TValue> : IResult
{
    TResult Match<TResult>(Func<TValue, TResult> success, Func<Error[], TResult> failure);
}