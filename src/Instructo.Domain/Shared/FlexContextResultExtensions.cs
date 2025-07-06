using MediatR;

namespace Domain.Shared;

public static class FlexContextResultExtensions
{
    public static Result<TOutput> Bind<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, Result<TOutput>> operation)
    {
        return input.IsError
            ? Result<TOutput>.Failure(input.Errors)
            : operation(input.Value!);
    }

    public static Result<Unit> Bind<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, Result<Unit>> operation)
    {
        return input.IsError
            ? Result<Unit>.Failure(input.Errors)
            : operation(input.Value!);
    }

    public static Result<FlexContext> Then<T>(
        this Result<FlexContext> contextResult,
        Func<FlexContext, Result<T>> operation)
    {
        if(contextResult.IsError)
            return Result<FlexContext>.Failure(contextResult.Errors);
        var context = contextResult.Value!;
        var opResult = operation(context);

        if(opResult.IsError)
            return Result<FlexContext>.Failure(opResult.Errors);

        context.TryAdd(opResult.Value!);
        return Result<FlexContext>.Success(context);
    }

    public static async Task<Result<FlexContext>> Then<T>(
        this Task<Result<FlexContext>> contextResult,
        Func<FlexContext, Task<Result<T>>> operation)
    {
        if((await contextResult).IsError)
            return Result<FlexContext>.Failure((await contextResult).Errors);
        var context = (await contextResult).Value!;
        var opResult = await operation(context);

        if(opResult is null||opResult.IsError)
            throw new Exception($"opResult is null on:{context} - {operation}");
        context.TryAdd(opResult.Value!);
        return Result<FlexContext>.Success(context);
    }

    public static async Task<Result<FlexContext>> Then<T>(
        this Task<Result<FlexContext>> contextResult,
        Func<FlexContext, Result<T>> operation)
    {
        if((await contextResult).IsError)
            return Result<FlexContext>.Failure((await contextResult).Errors);
        var context = (await contextResult).Value!;
        var opResult = operation(context);

        if(opResult.IsError)
            return Result<FlexContext>.Failure(opResult.Errors);
        context.TryAdd(opResult.Value!);
        return Result<FlexContext>.Success(context);
    }

    public static Task<Result<FlexContext>> Then<T>(
        this Result<FlexContext> contextResult,
        Func<FlexContext, Task<Result<T>>> operation)
    {
        if(contextResult.IsError)
            return Task.FromResult(Result<FlexContext>.Failure(contextResult.Errors));
        return ThenInternal(contextResult.Value!, operation);
    }

    private static async Task<Result<FlexContext>> ThenInternal<T>(
        FlexContext context,
        Func<FlexContext, Task<Result<T>>> operation)
    {
        var opResult = await operation(context);

        if(opResult.IsError)
            return Result<FlexContext>.Failure(opResult.Errors);
        context.TryAdd(opResult.Value!);
        return Result<FlexContext>.Success(context);
    }

    public static async Task<Result<bool>> FinalizeContext(
        this Task<Result<FlexContext>> contextResult,
        Action<FlexContext> operation)
    {
        if((await contextResult).IsError)
            return Result<bool>.Failure((await contextResult).Errors);

        var context = (await contextResult).Value!;
        operation(context);

        return Result<bool>.Success(default!);
    }

    public static async Task<Result<T>> MapContext<T>(
        this Task<Result<FlexContext>> contextResult,
        Func<FlexContext, Task<Result<T>>> operation)
    {
        if((await contextResult).IsError)
            return Result<T>.Failure((await contextResult).Errors);

        var context = (await contextResult).Value!;
        var opResult = await operation(context);

        if(opResult.IsError)
            return Result<T>.Failure(opResult.Errors);
        return Result<T>.Success(opResult.Value!);
    }

    public static Result<T> MapContext<T>(
        this Result<FlexContext> contextResult,
        Func<FlexContext, Result<T>> operation)
    {
        if(contextResult.IsError)
            return Result<T>.Failure(contextResult.Errors);

        var context = contextResult.Value!;
        var opResult = operation(context);

        if(opResult.IsError)
            return Result<T>.Failure(opResult.Errors);
        return Result<T>.Success(opResult.Value!);
    }
}