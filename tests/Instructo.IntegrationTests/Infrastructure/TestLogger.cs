using Infrastructure.Data.Repositories.Queries;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Instructo.IntegrationTests.Infrastructure;

public class TestLogger(ITestOutputHelper testOutputHelper) : ILogger<SchoolQueriesRepository>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null; // No scope management needed for this test logger
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true; // Always enabled for testing purposes
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        if(exception!=null)
        {
            testOutputHelper.WriteLine($"{logLevel}: {message} - Exception: {exception.Message}");
        }
        else
        {
            testOutputHelper.WriteLine($"{logLevel}: {message}");
        }
    }
}
