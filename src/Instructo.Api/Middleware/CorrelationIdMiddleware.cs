using System.Diagnostics;

using Serilog.Context;

namespace Api.Middleware;
public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string _correlationIdHeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[_correlationIdHeaderName].FirstOrDefault()
            ??Activity.Current?.TraceId.ToString()??Guid.NewGuid().ToString();

        // Add or update the correlation ID in the response headers
        context.Response.Headers[_correlationIdHeaderName]=correlationId;

        // Add to log context
        using(LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Add the correlation ID to the current activity
            Activity.Current?.SetTag("correlation_id", correlationId);

            await next(context);
        }
    }
}
