using Domain.Entities;

namespace Api.Middleware;

public class RoleBasedAuthorizationMiddleware(RequestDelegate next)
{
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

        await next(context);
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