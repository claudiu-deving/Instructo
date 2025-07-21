using Infrastructure.Data;

namespace Api.Builder;

public static class AppConfigurator
{
    public static void ConfigureCaching(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
    }
    public static void ConfigureRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<IpRateLimitOptions>(
            builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(
            builder.Configuration.GetSection("IpRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        builder.Services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval=TimeSpan.FromMinutes(30); // Default is 30 min
        });
    }

    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("GetorPost",
                builder =>
                    builder.AllowAnyOrigin().WithMethods("GET", "POST")
                        .AllowAnyHeader());
        });
    }

    public static void ConfigureSeeding(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SeedingOptions>(builder.Configuration.GetSection(SeedingOptions.Section));
    }

    public static void EnrichDiagonsticsWithUserIdInfo(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext=(diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value??"");
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

                if(httpContext.User.Identity?.IsAuthenticated!=true)
                    return;
                var value = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(value!=null)
                    diagnosticContext.Set("UserId",
                        value);
            };
        });
    }
}
