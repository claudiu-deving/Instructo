using System.Reflection;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

using Domain.Shared;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.Dtos.School;
using Domain.Entities.SchoolEntities;

namespace Instructo.Tests.Common;

public abstract class BaseTest
{
    protected ILogger logger;

    protected UserBuilder UserBuilder { get; } = new UserBuilder();

    protected BaseTest()
    {
        logger=new LoggerFactory().CreateLogger("TestLogger");
    }

    public ILogger<T> CreateLogger<T>()
    {
        return new TestLogger<T>(logger);
    }
}

public class TestLogger<T> : ILogger<T>
{
    private readonly ILogger _baseLogger;

    public TestLogger(ILogger baseLogger)
    {
        _baseLogger=baseLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => _baseLogger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => _baseLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        // You can add the type name as a prefix to distinguish between different loggers
        var typeName = typeof(T).Name;
        var message = formatter(state, exception);

        _baseLogger.Log(logLevel, eventId, state, exception, (s, e) => $"[{typeName}] {message}");
    }
}

/// <summary>
/// Custom AutoFixture attribute that provides auto-mocked dependencies
/// </summary>
public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}

/// <summary>
/// Custom AutoFixture attribute that allows some parameters to be specified manually
/// </summary>
public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new AutoMoqDataAttribute(), values)
    {
    }
}

/// <summary>
/// Customization for the Result<T> type to make AutoFixture work with it
/// </summary>
public class ResultCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => Result<School>.Success(fixture.Create<School>()));
        fixture.Register(() => Result<ApplicationUser>.Success(fixture.Create<ApplicationUser>()));
        fixture.Register(() => Result<SchoolReadDto>.Success(fixture.Create<SchoolReadDto>()));
    }
}

/// <summary>
/// Helper for testing private methods
/// </summary>
public static class PrivateMethodInvoker
{
    public static TResult InvokePrivateMethod<TResult>(object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName,
            BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy);

        if(method==null)
            throw new InvalidOperationException($"Method {methodName} not found on {type.Name}");

        var result = method.Invoke(instance, parameters);

        if(result is Task<TResult> task)
            return task.GetAwaiter().GetResult();

        return (TResult)result;
    }

    public static async Task<TResult> InvokePrivateMethodAsync<TResult>(object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName,
            BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy);

        if(method==null)
            throw new InvalidOperationException($"Method {methodName} not found on {type.Name}");

        var result = method.Invoke(instance, parameters);

        if(result is Task<TResult> task)
            return await task;

        return (TResult)result;
    }
}


/// <summary>
/// Extension methods for Result types to make testing easier
/// </summary>
public static class ResultTestExtensions
{
    public static Result<T> WithError<T>(this Result<T> result, string code, string message)
    {
        return Result<T>.Failure(new[] { new Error(code, message) });
    }

    public static void ShouldBeSuccessful<T>(this Result<T> result)
    {
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }

    public static void ShouldBeFailureWithError<T>(this Result<T> result, string errorCode)
    {
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code==errorCode);
    }
}