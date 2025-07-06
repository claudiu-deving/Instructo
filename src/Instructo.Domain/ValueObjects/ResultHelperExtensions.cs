using Domain.Shared;
namespace Domain.ValueObjects;

public class ResultHelperExtensions
{
    public static Result<T> Failure<T>(string value, string message)
    {
        return Result<T>.Failure([new Error(message, $"{value} for {typeof(T).Name}")]);
    }
}
