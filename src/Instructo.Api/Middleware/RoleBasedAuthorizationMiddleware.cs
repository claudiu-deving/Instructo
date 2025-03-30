using Instructo.Domain.Entities;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Instructo.Api.Middleware;

public class RoleBasedAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedAuthorizationMiddleware(RequestDelegate next)
    {
        _next=next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if(context.User.Identity?.IsAuthenticated==true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);

                if(user!=null)
                {
                    // Update LastLogin when user makes authenticated requests
                    user.LastLogin=DateTime.UtcNow;
                    await userManager.UpdateAsync(user);
                }
            }
        }

        await _next(context);
    }
}

// Extension method to make it easy to add the middleware
public static class RoleBasedAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleBasedAuthorization(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleBasedAuthorizationMiddleware>();
    }
}