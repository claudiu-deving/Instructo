using Domain.Entities;

namespace Api.Builder;

public static class AuthorizationBuilder
{
    public static void AddAuthorization(this WebApplicationBuilder builder)
    {
        var ironManId = GetIronManId();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(ApplicationRole.IronMan.Name!, policy => policy.RequireRole(
                    ApplicationRole.IronMan.Name!)
                .RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    ironManId))
            .AddPolicy(ApplicationRole.Admin.Name!, policy => policy.RequireRole(
                ApplicationRole.Admin.Name!,
                ApplicationRole.Owner.Name!,
                ApplicationRole.IronMan.Name!))
            .AddPolicy(ApplicationRole.Owner.Name!, policy => policy.RequireRole(
                ApplicationRole.Owner.Name!,
                ApplicationRole.IronMan.Name!))
            .AddPolicy(ApplicationRole.User.Name!, policy => policy.RequireRole(
                ApplicationRole.User.Name!,
                ApplicationRole.Instructor.Name!,
                ApplicationRole.Admin.Name!,
                ApplicationRole.Owner.Name!,
                ApplicationRole.IronMan.Name!));
    }

    public static string GetIronManId()
    {
#if DEBUG
        return "8fb090d2-ad97-41d0-86ce-08ddc4a5a731";
#endif
        var s = Environment.GetEnvironmentVariable("IRONMAN_ID");
        if(s is not null)
            return s;
        else
        {
            throw new InvalidOperationException("IRONMAN_ID environment variable is not set.");
        }
    }
}

