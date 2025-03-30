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
            var userAgent = context.Request.Headers.UserAgent.ToString();

            if(!user.Identity?.IsAuthenticated??true)
            {
                _logger.LogWarning(
                    "Unauthenticated access attempt to SuperUser endpoint {Path}. IP: {IpAddress}, UserAgent: {UserAgent}",
                    context.Request.Path, ipAddress, userAgent);
            }
            else if(!user.HasClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "13073b69-393f-4c5b-b96c-26b5d544d69e"))
            {
                _logger.LogWarning(
                    "Unauthorized access attempt to SuperUser endpoint {Path} by user {UserId} ({UserName}). IP: {IpAddress}",
                    context.Request.Path, userId, userName, ipAddress);
            }
            else
            {
                _logger.LogInformation(
                    "SuperUser {UserId} ({UserName}) accessed {Method} {Path}. IP: {IpAddress}",
                    userId, userName, context.Request.Method, context.Request.Path, ipAddress);
            }
        }

        await _next(context);
    }
}
