using Api.Builder;
using Api.Endpoints;
using Api.Middleware;

namespace Api.Config;

public static class AppComposer
{
    public static async Task<WebApplication> Compose(this WebApplication app)
    {
        var isAspireEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USING_ASPIRE"));

        if(isAspireEnabled)
            app.MapDefaultEndpoints();

        await app.InitializeDatabaseAsync(
               ToEnvironmentType(app.Environment.EnvironmentName));
        app.MapUserEndpoints();
        app.MapSchoolEndpoints();
        app.MapAuthEndpoints();

        if(app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseCors("GetorPost");
            app.UseMiniProfiler();
        }

        app.UseExceptionHandler(exceptionHandlerApp
            => exceptionHandlerApp.Run(async context
                => await Results.Problem().ExecuteAsync(context)));

        app.EnrichDiagonsticsWithUserIdInfo();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseMiddleware<IronmanAccessLoggingMiddleware>();

        app.UseRoleBasedAuthorization();

        app.UseIpRateLimiting();
        return app;
    }

    public static EnvironmentType ToEnvironmentType(string environmentName)
    {
        return environmentName switch
        {
            "Development" => EnvironmentType.Development,
            "Staging" => EnvironmentType.Staging,
            "Production" => EnvironmentType.Production,
            "Testing" => EnvironmentType.Testing,
            _ => throw new ArgumentException("Unknown environment type")
        };
    }
}
