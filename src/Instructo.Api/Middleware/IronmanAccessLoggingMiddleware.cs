using System.Security.Claims;

namespace Instructo.Api.Middleware;

public class IronmanAccessLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IronmanAccessLoggingMiddleware> _logger;

    public IronmanAccessLoggingMiddleware(RequestDelegate next, ILogger<IronmanAccessLoggingMiddleware> logger)
    {
        _next=next;
        _logger=logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if it's a superuser endpoint
        if(context.Request.Path.StartsWithSegments("/api/users"))
        {

            var user = context.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); 
            var userName = user.FindFirstValue(ClaimTypes.Name);  
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

         
                _logger.LogInformation(
                    "SuperUser {UserId} ({UserName}) accessed {Method} {Path}. IP: {IpAddress}",
                    userId, userName, context.Request.Method, context.Request.Path, ipAddress);
            
        }

        await _next(context);
    }
}
