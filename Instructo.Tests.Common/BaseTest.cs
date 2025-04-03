using Microsoft.Extensions.Logging;

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